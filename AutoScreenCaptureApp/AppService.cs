namespace AutoScreenCaptureApp;

public class AppService : IAppService
{
    public void Shutdown()
    {
        Application.Current.Shutdown(1);
    }
}