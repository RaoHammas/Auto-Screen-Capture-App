namespace AutoScreenCaptureApp.Services;

public class FolderPickerService : IFolderPickerService
{
    public string PickFolder()
    {
        using var dialog = new FolderBrowserDialog();
        var result = dialog.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
        {
            return dialog.SelectedPath;
        }

        return "";
    }
}