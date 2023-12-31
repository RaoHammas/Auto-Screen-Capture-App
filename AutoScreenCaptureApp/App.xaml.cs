﻿namespace AutoScreenCaptureApp;

/// <summary>
/// Interaction logic for AppService.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceProvider = ConfigureServices();
        var window = new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<IMainWindowViewModel>()
        };
        window.Show();
        base.OnStartup(e);
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAppService, AppService>();
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton<IAppHideShowService, AppHideShowService>();
        services.AddSingleton<IFolderPickerService, FolderPickerService>();
        services.AddSingleton<IDesktopCaptureService, DesktopCaptureService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IGlobalHotKeysService, GlobalHotKeysService>();
        services.AddSingleton<IFileOperationsService, FileOperationsService>();
        services.AddSingleton<IProcessService, ProcessService>();

        return services.BuildServiceProvider();
    }
}