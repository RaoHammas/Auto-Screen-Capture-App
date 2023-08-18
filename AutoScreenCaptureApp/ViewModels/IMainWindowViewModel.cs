namespace AutoScreenCaptureApp.ViewModels;

public interface IMainWindowViewModel
{
    public int IntervalTime { get; set; }
    public string SavePath { get; set; }
    public bool IsCapturing { get; set; }
    public bool IsAppVisible { get; set; }
    public int CaptureCount { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }

    Task ToggleCapturingAsync();
    void SelectSavePath();
    public void ToggleShowHide();
    public Task NavigateToGithub();
}