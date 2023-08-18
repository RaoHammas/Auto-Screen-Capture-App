namespace AutoScreenCaptureApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    private readonly IAppService _appService;
    private readonly IFolderPickerService _folderPickerService;
    private readonly IAppHideShowService _appHideShowService;
    private readonly IDesktopCaptureService _desktopCaptureService;
    private readonly IDialogService _dialogService;
    private readonly IGlobalHotKeysService _globalHotKeysService;
    private readonly IProcessService _processService;
    private readonly IFileOperationsService _fileOperationsService;

    [ObservableProperty] private int _intervalTime;
    [ObservableProperty] private string _savePath;
    [ObservableProperty] private bool _isCapturing;
    [ObservableProperty] private int _captureCount;
    [ObservableProperty] private bool _isAppVisible;
    [ObservableProperty] private CancellationTokenSource _cancellationTokenSource;

    public MainWindowViewModel(
        IFolderPickerService folderPickerService,
        IAppHideShowService appHideShowService,
        IDesktopCaptureService desktopCaptureService,
        IDialogService dialogService,
        IGlobalHotKeysService globalHotKeysService,
        IProcessService processService,
        IAppService appService,
        IFileOperationsService fileOperationsService
    )
    {
        _folderPickerService = folderPickerService;
        _appHideShowService = appHideShowService;
        _desktopCaptureService = desktopCaptureService;
        _dialogService = dialogService;
        _globalHotKeysService = globalHotKeysService;
        _processService = processService;
        _appService = appService;
        _fileOperationsService = fileOperationsService;
        CancellationTokenSource = new CancellationTokenSource();

        SavePath = fileOperationsService.GetSystemFolderPath(SpecialFolder.Desktop);
        IntervalTime = 10;
        IsCapturing = false;
        CaptureCount = 0;
        IsAppVisible = true;
    }

    [RelayCommand]
    public async Task ToggleCapturingAsync()
    {
        try
        {
            if (IsCapturing)
            {
                CancellationTokenSource.Cancel();
                IsCapturing = false;
            }
            else
            {
                if (!_fileOperationsService.Exists(SavePath))
                {
                    await _dialogService.ShowMessage("Invalid save path.");
                    return;
                }

                if (IntervalTime <= 0)
                {
                    IntervalTime = 1;
                }

                IsCapturing = true;

                CancellationTokenSource = new CancellationTokenSource();
                    var progress = new Progress<int>(count => { CaptureCount = count; });
                _ = _desktopCaptureService.StartCaptureAsync(TimeSpan.FromSeconds(IntervalTime), SavePath,
                    progress, CancellationTokenSource.Token);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessage(ex.Message);
            IsCapturing = false;
        }
    }

    [RelayCommand]
    public void SelectSavePath()
    {
        SavePath = _folderPickerService.PickFolder();
    }


    [RelayCommand]
    public void ToggleShowHide()
    {
        if (!IsAppVisible)
        {
            _appHideShowService.ShowApplication();
            _globalHotKeysService.UnRegisterShowAppHotKey();
            IsAppVisible = true;
        }
        else
        {
            _dialogService.ShowMessage(
                "AppService is now hidden from the Task-bar and Tab-Menu.\n To bring it back press Ctrl + Alt + Enter");

            _appHideShowService.HideApplication();
            _globalHotKeysService.RegisterShowAppHotKey(ToggleShowHide);
            IsAppVisible = false;
        }
    }

    [RelayCommand]
    public void CloseApp()
    {
        CancellationTokenSource.Cancel();
        _appService.Shutdown();
    }

    [RelayCommand]
    public async Task NavigateToGithub()
    {
        try
        {
            _processService.Start("https://github.com/RaoHammas");
        }
        catch (InvalidOperationException ex)
        {
            await _dialogService.ShowMessage(ex.Message);
        }
    }
}