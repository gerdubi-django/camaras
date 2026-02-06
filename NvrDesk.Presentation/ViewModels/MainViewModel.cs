using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using NvrDesk.Application.Abstractions;
using NvrDesk.Application.Dtos;
using NvrDesk.Domain.Contracts;
using NvrDesk.Domain.Enums;
using NvrDesk.Presentation.Services;
using System.Collections.ObjectModel;

namespace NvrDesk.Presentation.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly INvrService nvrService;
    private readonly IAuditRepository auditRepository;
    private readonly LibVLC libVlc;

    [ObservableProperty] private NvrDto? selectedNvr;
    [ObservableProperty] private string statusMessage = "Listo";
    [ObservableProperty] private int cameraGridSize = 4;
    [ObservableProperty] private DateTime? playbackStartDate = DateTime.Today;
    [ObservableProperty] private DateTime? playbackEndDate = DateTime.Today;
    [ObservableProperty] private string playbackStartTime = "08:00";
    [ObservableProperty] private string playbackEndTime = "09:00";
    [ObservableProperty] private ChannelItemViewModel? selectedPlaybackChannel;

    public ObservableCollection<NvrDto> Nvrs { get; } = [];
    public ObservableCollection<ChannelItemViewModel> Channels { get; } = [];
    public ObservableCollection<CameraCellViewModel> CameraCells { get; } = [];
    public ObservableCollection<RecordingSegmentDto> RecordingSegments { get; } = [];
    public CameraCellViewModel PlaybackCell { get; }

    public int GridColumns => (int)Math.Sqrt(CameraGridSize);
    public int GridRows => (int)Math.Sqrt(CameraGridSize);

    public MainViewModel(INvrService nvrService, IAuditRepository auditRepository)
    {
        this.nvrService = nvrService;
        this.auditRepository = auditRepository;
        libVlc = new LibVLC("--network-caching=400", "--rtsp-tcp", "--avcodec-hw=d3d11va");
        var resourceManager = new StreamResourceManager(16);
        PlaybackCell = new CameraCellViewModel(libVlc, auditRepository, resourceManager);
        FillCameraCells(resourceManager);
    }

    public async Task InitializeAsync()
    {
        await RefreshAsync();
    }

    partial void OnCameraGridSizeChanged(int value)
    {
        OnPropertyChanged(nameof(GridColumns));
        OnPropertyChanged(nameof(GridRows));
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Nvrs.Clear();
        foreach (var item in await nvrService.ListNvrsAsync()) Nvrs.Add(item);

        Channels.Clear();
        foreach (var channel in await nvrService.ListAllChannelsAsync()) Channels.Add(new ChannelItemViewModel(channel));

        StatusMessage = "Datos actualizados.";
    }

    [RelayCommand]
    private async Task AddNvrAsync()
    {
        var dto = new NvrDto
        {
            Name = "Nuevo NVR",
            Brand = BrandType.Hikvision,
            Host = "192.168.1.100",
            HttpPort = 80,
            RtspPort = 554,
            Username = "admin",
            Password = "admin123"
        };

        var result = await nvrService.SaveNvrAsync(dto);
        StatusMessage = result.Success ? "NVR agregado. Edite luego los datos reales." : string.Join(Environment.NewLine, result.Errors);
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task DeleteSelectedNvrAsync()
    {
        if (SelectedNvr is null) return;
        await nvrService.DeleteNvrAsync(SelectedNvr.Id);
        await RefreshAsync();
        StatusMessage = "NVR eliminado.";
    }

    [RelayCommand]
    private async Task TestSelectedNvrAsync()
    {
        if (SelectedNvr is null) return;
        var result = await nvrService.TestConnectionAsync(SelectedNvr.Id);
        StatusMessage = result.Message;
    }

    [RelayCommand]
    private async Task SyncSelectedNvrChannelsAsync()
    {
        if (SelectedNvr is null) return;
        var count = await nvrService.SyncChannelsAsync(SelectedNvr.Id);
        StatusMessage = $"Sincronizados {count} canales.";
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task SearchRecordingsAsync()
    {
        if (SelectedPlaybackChannel is null || PlaybackStartDate is null || PlaybackEndDate is null) return;
        var start = DateTime.Parse($"{PlaybackStartDate:yyyy-MM-dd} {PlaybackStartTime}");
        var end = DateTime.Parse($"{PlaybackEndDate:yyyy-MM-dd} {PlaybackEndTime}");
        var segments = await nvrService.SearchRecordingsAsync(SelectedPlaybackChannel.Id, start, end);
        RecordingSegments.Clear();
        foreach (var segment in segments) RecordingSegments.Add(segment);
        StatusMessage = $"Se encontraron {segments.Count} segmentos.";
    }

    [RelayCommand]
    private async Task PlayRecordingAsync()
    {
        if (SelectedPlaybackChannel is null || PlaybackStartDate is null || PlaybackEndDate is null) return;
        var start = DateTime.Parse($"{PlaybackStartDate:yyyy-MM-dd} {PlaybackStartTime}");
        var end = DateTime.Parse($"{PlaybackEndDate:yyyy-MM-dd} {PlaybackEndTime}");
        var url = await nvrService.BuildPlaybackUrlAsync(SelectedPlaybackChannel.Id, start, end);
        await PlaybackCell.PlayAsync(url, SelectedPlaybackChannel.DisplayName, CancellationToken.None);
    }

    private void FillCameraCells(StreamResourceManager resourceManager)
    {
        CameraCells.Clear();
        for (var i = 0; i < 16; i++) CameraCells.Add(new CameraCellViewModel(libVlc, auditRepository, resourceManager));
    }
}
