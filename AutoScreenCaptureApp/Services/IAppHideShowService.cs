namespace AutoScreenCaptureApp.Services;

public interface IAppHideShowService
{
    void HideApplication(object window);
    void ShowApplication(object window);
}