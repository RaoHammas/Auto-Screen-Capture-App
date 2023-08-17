namespace AutoScreenCaptureApp;

public partial class MainWindowViewModel : ObservableObject, IMainWindowViewModel
{
    private readonly IFolderPickerService _folderPickerService;
    private readonly IAppHideShowService _appHideShowService;
    private readonly IDesktopCaptureService _desktopCaptureService;
    private readonly IDialogService _dialogService;
    [ObservableProperty] private int _intervalTime;
    [ObservableProperty] private string _savePath;
    [ObservableProperty] private bool _isCapturing;
    [ObservableProperty] private int _captureCount;

    private CancellationTokenSource _cancellationTokenSource;

    public MainWindowViewModel(
        IFolderPickerService folderPickerService,
        IAppHideShowService appHideShowService,
        IDesktopCaptureService desktopCaptureService,
        IDialogService dialogService
    )
    {
        _folderPickerService = folderPickerService;
        _appHideShowService = appHideShowService;
        _desktopCaptureService = desktopCaptureService;
        _dialogService = dialogService;
        _cancellationTokenSource = new CancellationTokenSource();

        SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        IntervalTime = 10;
        IsCapturing = false;
        CaptureCount = 0;

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
                    return;
                }

                if (IntervalTime <= 0)
                {
                    IntervalTime = 1;
                }

                IsCapturing = true;

                _cancellationTokenSource = new CancellationTokenSource();
                var progress = new Progress<int>(count => { CaptureCount = count; });
                await _desktopCaptureService.StartCaptureAsync(TimeSpan.FromSeconds(IntervalTime), SavePath,
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
    public void HideApp(object win)
    {
        //_appHideShowService.HideApplication(win);
    }

    [RelayCommand]
    public void CloseApp()
    {
        Environment.Exit(0);
    }

    [RelayCommand]
    public async Task NavigateToGithub()
    {
        try
        {
            Process myProcess = new Process();
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