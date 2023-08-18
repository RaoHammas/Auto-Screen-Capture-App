namespace AutoScreenCaptureApp.Services;

public class FileOperationsService : IFileOperationsService
{
    public bool Exists(string path)
    {
        return Directory.Exists(path);
    }

    public string GetSystemFolderPath(SpecialFolder folder)
    {
        return GetFolderPath(folder);
    }
}