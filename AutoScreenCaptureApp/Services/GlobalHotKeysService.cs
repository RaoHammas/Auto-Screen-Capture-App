namespace AutoScreenCaptureApp.Services;

public class GlobalHotKeysService : IGlobalHotKeysService
{
    public void RegisterShowAppHotKey(Action action)
    {
        HotkeyManager.Current.AddOrReplace("AutoSSShowKey", Key.Enter, ModifierKeys.Control | ModifierKeys.Alt,
            (_, _) => action.Invoke());
    }

    public void UnRegisterShowAppHotKey()
    {
        HotkeyManager.Current.Remove("AutoSSShowKey");
    }
}