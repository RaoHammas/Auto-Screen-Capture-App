namespace AutoScreenCaptureApp;

public interface IMainWindowViewModel
{
    public int IntervalTime { get; set; }
    public string SavePath { get; set; }
    public bool IsCapturing { get; set; }
    public int CaptureCount { get; set; }

    Task ToggleCapturingAsync();
    void SelectSavePath();
    public void HideApp(object win);
    public Task NavigateToGithub();
}