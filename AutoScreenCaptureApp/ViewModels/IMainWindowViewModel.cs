namespace AutoScreenCaptureApp.ViewModels;

public interface IMainWindowViewModel
{
    public int IntervalTime { get; set; }
    public string SavePath { get; set; }
    public bool IsCapturing { get; set; }
    public bool IsAppVisible { get; set; }
    public int CaptureCount { get; set; }

    Task ToggleCapturingAsync();
    void SelectSavePath();
    public void ToggleShowHide(object win);
    public Task NavigateToGithub();
}