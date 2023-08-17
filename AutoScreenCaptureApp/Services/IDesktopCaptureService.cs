namespace AutoScreenCaptureApp.Services;

public interface IDesktopCaptureService
{
    Task StartCaptureAsync(TimeSpan interval, string savePath, IProgress<int> progress,
        CancellationToken cancellationToken);
}