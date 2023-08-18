namespace AutoScreenCaptureApp.Services;

public interface IGlobalHotKeysService
{
    void RegisterShowAppHotKey(Action action);
    void UnRegisterShowAppHotKey();
}