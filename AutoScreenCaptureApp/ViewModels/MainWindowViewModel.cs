namespace AutoScreenCaptureApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    private readonly IFolderPickerService _folderPickerService;
    private readonly IAppHideShowService _appHideShowService;
    private readonly IDesktopCaptureService _desktopCaptureService;
    private readonly IDialogService _dialogService;
    private readonly IGlobalHotKeysService _globalHotKeysService;
    [ObservableProperty] private int _intervalTime;
    [ObservableProperty] private string _savePath;
    [ObservableProperty] private bool _isCapturing;
    [ObservableProperty] private int _captureCount;
    [ObservableProperty] private bool _isAppVisible;

    private CancellationTokenSource _cancellationTokenSource;

    public MainWindowViewModel(
        IFolderPickerService folderPickerService,
        IAppHideShowService appHideShowService,
        IDesktopCaptureService desktopCaptureService,
        IDialogService dialogService,
        IGlobalHotKeysService globalHotKeysService
    )
    {
        _folderPickerService = folderPickerService;
        _appHideShowService = appHideShowService;
        _desktopCaptureService = desktopCaptureService;
        _dialogService = dialogService;
        _globalHotKeysService = globalHotKeysService;
        _cancellationTokenSource = new CancellationTokenSource();

        SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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
                _cancellationTokenSource.Cancel();
                IsCapturing = false;
            }
            else
            {
                if (!Directory.Exists(SavePath))
                {
                    await _dialogService.ShowMessage("Invalid save path.");
                    return;
                }

                if (IntervalTime <= 0)
                {
                    IntervalTime = 1;
                }

                IsCapturing = true;

                _cancellationTokenSource = new CancellationTokenSource();
                var progress = new Progress<int>(count => { CaptureCount = count; });
                _ = _desktopCaptureService.StartCaptureAsync(TimeSpan.FromSeconds(IntervalTime), SavePath,
                    progress, _cancellationTokenSource.Token);
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
    public void ToggleShowHide(object win)
    {
        if (!IsAppVisible)
        {
            _appHideShowService.ShowApplication(win);
            _globalHotKeysService.UnRegisterShowAppHotKey();
            IsAppVisible = true;
        }
        else
        {
            _dialogService.ShowMessage(
                "App is now hidden from the Task-bar and Tab-Menu.\n To bring it back press Ctrl + Alt + Enter");

            _appHideShowService.HideApplication(win);
            _globalHotKeysService.RegisterShowAppHotKey(ToggleShowHideCommand, win);
            IsAppVisible = false;
        }
    }

    [RelayCommand]
    public void CloseApp()
    {
        _cancellationTokenSource.Cancel();
        Environment.Exit(0);
    }

    [RelayCommand]
    public async Task NavigateToGithub()
    {
        try
        {
            var myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = "https://github.com/RaoHammas";
            myProcess.Start();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowMessage(ex.Message);
        }
    }
}