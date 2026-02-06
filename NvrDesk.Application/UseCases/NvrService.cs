using NvrDesk.Application.Abstractions;
using NvrDesk.Application.Dtos;
using NvrDesk.Application.Validation;
using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Entities;
using NvrDesk.Domain.Enums;
using NvrDesk.Domain.ValueObjects;

namespace NvrDesk.Application.UseCases;

public sealed class NvrService(
    INvrRepository nvrRepository,
    IChannelRepository channelRepository,
    IAuditRepository auditRepository,
    INvrDriverFactory driverFactory,
    IEncryptionService encryptionService) : INvrService
{
    public async Task<IReadOnlyList<NvrDto>> ListNvrsAsync(CancellationToken cancellationToken = default)
    {
        var nvrs = await nvrRepository.ListAsync(cancellationToken);
        return nvrs.Select(x => new NvrDto
        {
            Id = x.Id,
            Name = x.Name,
            Brand = x.Brand,
            Host = x.Host,
            HttpPort = x.HttpPort,
            RtspPort = x.RtspPort,
            Username = x.Username,
            Password = encryptionService.Decrypt(x.EncryptedPassword)
        }).ToList();
    }

    public async Task<(bool Success, IReadOnlyList<string> Errors)> SaveNvrAsync(NvrDto dto, CancellationToken cancellationToken = default)
    {
        var errors = NvrValidator.Validate(dto);
        if (errors.Count != 0) return (false, errors);

        if (dto.Id == Guid.Empty)
        {
            var entity = new NvrDevice
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Host = dto.Host,
                HttpPort = dto.HttpPort,
                RtspPort = dto.RtspPort,
                Username = dto.Username,
                EncryptedPassword = encryptionService.Encrypt(dto.Password)
            };
            await nvrRepository.AddAsync(entity, cancellationToken);
        }
        else
        {
            var existing = await nvrRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existing is null) return (false, ["NVR no encontrado."]);
            existing.Name = dto.Name;
            existing.Brand = dto.Brand;
            existing.Host = dto.Host;
            existing.HttpPort = dto.HttpPort;
            existing.RtspPort = dto.RtspPort;
            existing.Username = dto.Username;
            existing.EncryptedPassword = encryptionService.Encrypt(dto.Password);
            existing.UpdatedAtUtc = DateTime.UtcNow;
            await nvrRepository.UpdateAsync(existing, cancellationToken);
        }

        return (true, []);
    }

    public Task DeleteNvrAsync(Guid nvrId, CancellationToken cancellationToken = default) => nvrRepository.DeleteAsync(nvrId, cancellationToken);

    public async Task<TestResult> TestConnectionAsync(Guid nvrId, CancellationToken cancellationToken = default)
    {
        var nvr = await GetNvr(nvrId, cancellationToken);
        var driver = driverFactory.Resolve(nvr.Brand);
        var result = await driver.TestConnectionAsync(ToConnection(nvr));
        await auditRepository.AddAsync(new AuditLogEntry
        {
            EventType = result.IsSuccess ? AuditEventType.Synchronization : AuditEventType.StreamError,
            Message = $"Test conexión NVR {nvr.Name}: {result.Message}"
        }, cancellationToken);
        return result;
    }

    public async Task<int> SyncChannelsAsync(Guid nvrId, CancellationToken cancellationToken = default)
    {
        var nvr = await GetNvr(nvrId, cancellationToken);
        var driver = driverFactory.Resolve(nvr.Brand);
        var channels = await driver.ListChannelsAsync(ToConnection(nvr));
        foreach (var channel in channels) channel.NvrDeviceId = nvrId;
        await channelRepository.ReplaceForNvrAsync(nvrId, channels, cancellationToken);
        await auditRepository.AddAsync(new AuditLogEntry
        {
            EventType = AuditEventType.Synchronization,
            Message = $"Sincronización de canales para {nvr.Name}: {channels.Count} canal(es)."
        }, cancellationToken);
        return channels.Count;
    }

    public async Task<IReadOnlyList<ChannelDto>> ListAllChannelsAsync(CancellationToken cancellationToken = default)
    {
        var channels = await channelRepository.ListAllAsync(cancellationToken);
        var nvrs = await nvrRepository.ListAsync(cancellationToken);
        return (from channel in channels
                join nvr in nvrs on channel.NvrDeviceId equals nvr.Id
                select new ChannelDto
                {
                    Id = channel.Id,
                    NvrDeviceId = channel.NvrDeviceId,
                    Number = channel.Number,
                    Name = channel.Name,
                    NvrName = nvr.Name
                }).ToList();
    }

    public async Task<string> BuildLiveUrlAsync(Guid channelId, StreamProfile profile, CancellationToken cancellationToken = default)
    {
        var (channel, nvr) = await ResolveChannelWithNvr(channelId, cancellationToken);
        return driverFactory.Resolve(nvr.Brand).BuildRtspLiveUrl(ToConnection(nvr), channel, profile);
    }

    public async Task<IReadOnlyList<RecordingSegmentDto>> SearchRecordingsAsync(Guid channelId, DateTime startLocal, DateTime endLocal, CancellationToken cancellationToken = default)
    {
        var (channel, nvr) = await ResolveChannelWithNvr(channelId, cancellationToken);
        var startUtc = startLocal.ToUniversalTime();
        var endUtc = endLocal.ToUniversalTime();
        var segments = await driverFactory.Resolve(nvr.Brand).SearchRecordingsAsync(ToConnection(nvr), channel, startUtc, endUtc);

        await auditRepository.AddAsync(new AuditLogEntry
        {
            EventType = AuditEventType.PlaybackSearch,
            Message = $"Búsqueda de grabaciones en cámara {channel.Name} ({nvr.Name})."
        }, cancellationToken);

        return segments.Select(x => new RecordingSegmentDto
        {
            StartUtc = x.StartUtc,
            EndUtc = x.EndUtc,
            Quality = x.Quality,
            Notes = x.Notes
        }).ToList();
    }

    public async Task<string> BuildPlaybackUrlAsync(Guid channelId, DateTime startLocal, DateTime endLocal, CancellationToken cancellationToken = default)
    {
        var (channel, nvr) = await ResolveChannelWithNvr(channelId, cancellationToken);
        return driverFactory.Resolve(nvr.Brand).BuildRtspPlaybackUrl(ToConnection(nvr), channel, startLocal.ToUniversalTime(), endLocal.ToUniversalTime());
    }

    private async Task<NvrDevice> GetNvr(Guid nvrId, CancellationToken cancellationToken)
    {
        var nvr = await nvrRepository.GetByIdAsync(nvrId, cancellationToken);
        return nvr ?? throw new InvalidOperationException("NVR no encontrado.");
    }

    private async Task<(Channel channel, NvrDevice nvr)> ResolveChannelWithNvr(Guid channelId, CancellationToken cancellationToken)
    {
        var channels = await channelRepository.ListAllAsync(cancellationToken);
        var channel = channels.FirstOrDefault(x => x.Id == channelId) ?? throw new InvalidOperationException("Canal no encontrado.");
        var nvr = await nvrRepository.GetByIdAsync(channel.NvrDeviceId, cancellationToken) ?? throw new InvalidOperationException("NVR de canal no encontrado.");
        return (channel, nvr);
    }

    private NvrConnection ToConnection(NvrDevice nvr) => new(nvr.Brand, nvr.Host, nvr.HttpPort, nvr.RtspPort, nvr.Username, encryptionService.Decrypt(nvr.EncryptedPassword));
}
