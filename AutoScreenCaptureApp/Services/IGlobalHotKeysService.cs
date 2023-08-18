namespace AutoScreenCaptureApp.Services;

public interface IGlobalHotKeysService
{
    void RegisterShowAppHotKey(ICommand command, object commandParam);
    void UnRegisterShowAppHotKey();
}