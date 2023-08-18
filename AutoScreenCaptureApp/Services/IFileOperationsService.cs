namespace AutoScreenCaptureApp.Services;

public interface IFileOperationsService
{
    bool Exists(string path);
    string GetSystemFolderPath(SpecialFolder folder);
}