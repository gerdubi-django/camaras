using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;
using NvrDesk.Presentation.Services;

namespace NvrDesk.Presentation.ViewModels;

public sealed partial class CameraCellViewModel : ObservableObject
{
    private readonly IAuditRepository auditRepository;
    private readonly StreamResourceManager resourceManager;
    private readonly int maxReconnectAttempts;

    public Guid StreamId { get; } = Guid.NewGuid();
    public MediaPlayer MediaPlayer { get; }

    [ObservableProperty] private string title = "Sin cámara";
    [ObservableProperty] private string status = "Sin conexión";

    public CameraCellViewModel(LibVLC libVlc, IAuditRepository auditRepository, StreamResourceManager resourceManager, int maxReconnectAttempts = 5)
    {
        this.auditRepository = auditRepository;
        this.resourceManager = resourceManager;
        this.maxReconnectAttempts = maxReconnectAttempts;
        MediaPlayer = new MediaPlayer(libVlc);
        MediaPlayer.EncounteredError += async (_, _) => await RegisterErrorAsync("Error en reproducción RTSP.");
    }

    public async Task PlayAsync(string streamUrl, string cameraTitle, CancellationToken cancellationToken)
    {
        Title = cameraTitle;
        if (!await resourceManager.TryAcquireAsync(StreamId, cancellationToken))
        {
            Status = "Límite de streams alcanzado";
            return;
        }

        var attempts = 0;
        while (attempts < maxReconnectAttempts)
        {
            attempts++;
            Status = attempts == 1 ? "Conectando..." : $"Reconectando ({attempts}/{maxReconnectAttempts})";
            using var media = new Media(MediaPlayer.LibVLC, streamUrl, FromType.FromLocation);
            if (MediaPlayer.Play(media))
            {
                Status = "Online";
                await auditRepository.AddAsync(new AuditLogEntry { EventType = AuditEventType.StreamStart, Message = $"Inicio stream {cameraTitle}" }, cancellationToken);
                return;
            }

            var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempts));
            await Task.Delay(backoff, cancellationToken);
        }

        await RegisterErrorAsync($"No se pudo conectar después de {maxReconnectAttempts} intentos.");
        resourceManager.Release(StreamId);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        MediaPlayer.Stop();
        Status = "Detenido";
        resourceManager.Release(StreamId);
        await auditRepository.AddAsync(new AuditLogEntry { EventType = AuditEventType.StreamStop, Message = $"Fin stream {Title}" }, cancellationToken);
    }

    private async Task RegisterErrorAsync(string message)
    {
        Status = "Error";
        await auditRepository.AddAsync(new AuditLogEntry { EventType = AuditEventType.StreamError, Message = message });
    }
}
