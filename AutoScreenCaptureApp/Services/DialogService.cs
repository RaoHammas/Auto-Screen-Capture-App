namespace AutoScreenCaptureApp.Services;

public class DialogService : IDialogService
{
    public async Task ShowMessage(string message)
    {
        var messageBox = new MessageDialogView
        {
            Message = { Text = message }
        };

        await DialogHost.Show(messageBox, "RootDialog");
    }
}