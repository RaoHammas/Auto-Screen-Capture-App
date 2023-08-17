using System.Windows;

namespace AutoScreenCaptureApp.Services;

public class AppHideShowService : IAppHideShowService
{
    public void HideApplication(object win)
    {
        if (win is Window window)
        {
            window.ShowInTaskbar = false;
            window.Visibility = Visibility.Hidden;
        }
    }

    public void ShowApplication(object win)
    {
        if (win is Window window)
        {
            window.ShowInTaskbar = true;
            window.Visibility = Visibility.Visible;
        }
    }
}