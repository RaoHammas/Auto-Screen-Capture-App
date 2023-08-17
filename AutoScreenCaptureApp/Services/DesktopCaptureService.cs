namespace AutoScreenCaptureApp.Services;

public class DesktopCaptureService : IDesktopCaptureService
{
    public async Task StartCaptureAsync(TimeSpan interval, string savePath, IProgress<int> progress,
        CancellationToken cancellationToken)
    {
        var screenshotCount = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var screenshot = CaptureScreen();
            var filePath = Path.Combine(savePath, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            screenshotCount++;
            progress.Report(screenshotCount);
            await Task.Delay(interval, cancellationToken);
        }

    }

    private Bitmap CaptureScreen()
    {
        var bounds = Screen.GetBounds(Point.Empty);
        var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

        return bitmap;
    }
}