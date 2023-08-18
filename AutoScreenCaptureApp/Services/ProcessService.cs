namespace AutoScreenCaptureApp.Services;

public class ProcessService : IProcessService
{
    public bool Start(string processName)
    {
        var myProcess = new Process();
        myProcess.StartInfo.UseShellExecute = true;
        myProcess.StartInfo.FileName = processName;
        myProcess.Start();

        return true;
    }
}