namespace AutoScreenCaptureApp.Tests;

[TestFixture]
public class MainWindowViewModelTests
{
    private IFolderPickerService _folderPickerService;
    private IAppHideShowService _appHideShowService;
    private IDesktopCaptureService _desktopCaptureService;
    private IDialogService _dialogService;
    private IGlobalHotKeysService _globalHotKeysService;
    private IProcessService _processService;
    private IAppService _appService;
    private IFileOperationsService _fileOperationsService;

    private MainWindowViewModel _viewModel;

    [SetUp]
    public void SetUp()
    {
        _folderPickerService = Substitute.For<IFolderPickerService>();
        _appHideShowService = Substitute.For<IAppHideShowService>();
        _desktopCaptureService = Substitute.For<IDesktopCaptureService>();
        _dialogService = Substitute.For<IDialogService>();
        _globalHotKeysService = Substitute.For<IGlobalHotKeysService>();
        _processService = Substitute.For<IProcessService>();
        _appService = Substitute.For<IAppService>();
        _fileOperationsService = Substitute.For<IFileOperationsService>();

        _viewModel = new MainWindowViewModel(
            _folderPickerService,
            _appHideShowService,
            _desktopCaptureService,
            _dialogService,
            _globalHotKeysService,
            _processService,
            _appService,
            _fileOperationsService
        )
        {
            CancellationTokenSource = new CancellationTokenSource()
        };
    }

    [TearDown]
    public void TearDown()
    {
        _viewModel.CancellationTokenSource.Dispose();
    }

    [Test]
    public async Task ToggleCapturingAsync_InvalidSavePath_ShowMessageAndCaptureNotStarted()
    {
        // Arrange
        _fileOperationsService.Exists(Arg.Any<string>()).Returns(false);
        _dialogService.ShowMessage(Arg.Any<string>()).Returns(Task.CompletedTask);

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.IsFalse(_viewModel.IsCapturing);
        await _dialogService.Received(1).ShowMessage("Invalid save path.");
        await _desktopCaptureService.DidNotReceive().StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(),
            Arg.Any<IProgress<int>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ToggleCapturingAsync_ValidSavePath_CapturingStarted()
    {
        // Arrange
        _fileOperationsService.Exists(Arg.Any<string>()).Returns(true);
        _folderPickerService.PickFolder().Returns("C:\\ValidPath");
        _desktopCaptureService.StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(), Arg.Any<IProgress<int>>(),
            Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.IsTrue(_viewModel.IsCapturing);
        await _dialogService.DidNotReceive().ShowMessage(Arg.Any<string>());
        await _desktopCaptureService.Received(1).StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(),
            Arg.Any<IProgress<int>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ToggleCapturingAsync_Stop_if_CapturingStarted()
    {
        // Arrange
        _viewModel.IsCapturing = true;
        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.IsFalse(_viewModel.IsCapturing);
        Assert.IsTrue(_viewModel.CancellationTokenSource.IsCancellationRequested);
    }

    [Test]
    public async Task ToggleCapturingAsync_Exception_ShowMessageAndCaptureNotStarted()
    {
        // Arrange
        _fileOperationsService.Exists(Arg.Any<string>()).Returns(true);
        _dialogService.ShowMessage(Arg.Any<string>()).Returns(Task.CompletedTask);
        _desktopCaptureService
            .StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(), Arg.Any<IProgress<int>>(),
                Arg.Any<CancellationToken>()).Throws(new Exception("Some error"));

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.IsFalse(_viewModel.IsCapturing);
        await _dialogService.Received(1).ShowMessage("Some error");
        await _desktopCaptureService.Received(1).StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(),
            Arg.Any<IProgress<int>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ToggleCapturingAsync_IntervalTimeLessThanOrEqualZero_IntervalTimeSetToOne()
    {
        // Arrange
        _fileOperationsService.Exists(Arg.Any<string>()).Returns(true);
        _viewModel.IntervalTime = 0;

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.That(_viewModel.IntervalTime, Is.EqualTo(1));
        _fileOperationsService.Received(1).Exists(Arg.Any<string>());
        await _desktopCaptureService.Received(1).StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(),
            Arg.Any<IProgress<int>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ToggleCapturingAsync_IntervalTimeGreaterThanZero_NoChangeToIntervalTime()
    {
        // Arrange
        _fileOperationsService.Exists(Arg.Any<string>()).Returns(true);
        _viewModel.IntervalTime = 5;

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        _fileOperationsService.Received(1).Exists(Arg.Any<string>());
        Assert.That(_viewModel.IntervalTime, Is.EqualTo(5));
        await _desktopCaptureService.Received(1).StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(),
            Arg.Any<IProgress<int>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ToggleCapturingAsync_CaptureCountIncreasedOnProgress()
    {
        // Arrange
        _viewModel.CaptureCount = 0;
        var cancellationTokenSource = new CancellationTokenSource();

        _fileOperationsService.Exists(Arg.Any<string>()).Returns(true);
        _viewModel.IntervalTime = 10;
        _desktopCaptureService
            .StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(), Arg.Any<IProgress<int>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo =>
            {
                var progressArg = callInfo.ArgAt<IProgress<int>>(2);
                progressArg.Report(5);
                cancellationTokenSource.Cancel();
            });

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.That(_viewModel.CaptureCount, Is.EqualTo(5));
    }


    [Test]
    public async Task ToggleCapturingAsync_ValidIntervalTime_ProgressInitializedAndCaptureStarted()
    {
        // Arrange
        _fileOperationsService.Exists(Arg.Any<string>()).Returns(true);
        _viewModel.IntervalTime = 10;

        // Act
        await _viewModel.ToggleCapturingAsync();

        // Assert
        Assert.IsTrue(_viewModel.IsCapturing);
        _fileOperationsService.Received().Exists(Arg.Any<string>());
        await _desktopCaptureService.Received(1).StartCaptureAsync(Arg.Any<TimeSpan>(), Arg.Any<string>(),
            Arg.Any<IProgress<int>>(), Arg.Any<CancellationToken>());
    }


    [Test]
    public void SelectSavePath_FolderPickerReturnsPath_PathSetCorrectly()
    {
        // Arrange
        const string selectedPath = "C:\\MySelectedPath";
        _folderPickerService.PickFolder().Returns(selectedPath);

        // Act
        _viewModel.SelectSavePath();

        // Assert
        Assert.That(_viewModel.SavePath, Is.EqualTo(selectedPath));
    }

    [Test]
    public void ToggleShowHide_AppVisible_HidesAppAndRegistersHotKey()
    {
        // Arrange
        _viewModel.IsAppVisible = true;

        // Act
        _viewModel.ToggleShowHide();

        // Assert
        _appHideShowService.Received(1).HideApplication();
        _globalHotKeysService.Received(1).RegisterShowAppHotKey(Arg.Any<Action>());
        Assert.IsFalse(_viewModel.IsAppVisible);
    }

    [Test]
    public void ToggleShowHide_AppHidden_ShowsAppAndUnRegistersHotKey()
    {
        // Arrange
        _viewModel.IsAppVisible = false;

        // Act
        _viewModel.ToggleShowHide();

        // Assert
        _appHideShowService.Received(1).ShowApplication();
        _globalHotKeysService.Received(1).UnRegisterShowAppHotKey();
        Assert.IsTrue(_viewModel.IsAppVisible);
    }

    [Test]
    public void CloseApp_CancelToken_ShutdownAppAndCancelsToken()
    {
        // Arrange
        _viewModel.IsCapturing = true;
        // Act
        _viewModel.CloseApp();

        // Assert
        Assert.IsTrue(_viewModel.CancellationTokenSource.IsCancellationRequested);
        _appService.Received(1).Shutdown();
    }

    [Test]
    public async Task NavigateToGithub_Success_ProcessStarted()
    {
        // Arrange
        _processService.Start(Arg.Any<string>()).Returns(true);

        // Act
        await _viewModel.NavigateToGithub();

        // Assert
        _processService.Received(1).Start("https://github.com/RaoHammas");
        await _dialogService.DidNotReceive().ShowMessage(Arg.Any<string>());
    }

    [Test]
    public async Task NavigateToGithub_Exception_ShowMessage()
    {
        // Arrange
        _processService.Start(Arg.Any<string>()).Throws(new InvalidOperationException("Process start failed"));
        _dialogService.ShowMessage(Arg.Any<string>()).Returns(Task.CompletedTask);

        // Act
        await _viewModel.NavigateToGithub();

        // Assert
        _processService.Received(1).Start(Arg.Any<string>());
        await _dialogService.Received(1).ShowMessage("Process start failed");
    }
}