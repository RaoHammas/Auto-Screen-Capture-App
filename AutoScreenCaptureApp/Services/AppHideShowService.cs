using System.Diagnostics.CodeAnalysis;

namespace AutoScreenCaptureApp.Services;

[ExcludeFromCodeCoverage]
public class AppHideShowService : IAppHideShowService
{
    public void HideApplication()
    {
        Application.Current.MainWindow!.ShowInTaskbar = false;
        Application.Current.MainWindow!.Visibility = Visibility.Hidden;
    }

    public void ShowApplication()
    {
        Application.Current.MainWindow!.ShowInTaskbar = true;
        Application.Current.MainWindow!.Visibility = Visibility.Visible;
    }
}