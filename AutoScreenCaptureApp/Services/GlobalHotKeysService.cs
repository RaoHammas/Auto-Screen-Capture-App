namespace AutoScreenCaptureApp.Services;

public class GlobalHotKeysService : IGlobalHotKeysService
{
    public void RegisterShowAppHotKey(ICommand command, object commandParam)
    {
        HotkeyManager.Current.AddOrReplace("AutoSSShowKey", Key.Enter, ModifierKeys.Control | ModifierKeys.Alt,
            (_, _) => command.Execute(commandParam));
    }

    public void UnRegisterShowAppHotKey()
    {
        HotkeyManager.Current.Remove("AutoSSShowKey");
    }
}