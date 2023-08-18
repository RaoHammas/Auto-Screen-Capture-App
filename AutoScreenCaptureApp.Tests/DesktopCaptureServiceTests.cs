namespace AutoScreenCaptureApp.Tests
{
    [TestFixture]
    public class DesktopCaptureServiceTests
    {
        private IDesktopCaptureService _desktopCaptureService;
        private IProgress<int> _progress;

        [SetUp]
        public void Setup()
        {
            _progress = Substitute.For<IProgress<int>>();
            _desktopCaptureService = new DesktopCaptureService();
        }

        [Test]
        public Task StartCaptureAsync_CapturesScreenshots()
        {
            // Arrange
            var interval = TimeSpan.FromMilliseconds(1); // Minimal interval for faster testing
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            _ = _desktopCaptureService.StartCaptureAsync(interval, savePath, _progress, cancellationToken);
            cancellationTokenSource.Cancel(); // Stop capturing

            // Assert
            _progress.ReceivedWithAnyArgs().Report(Arg.Any<int>()); // Ensure progress was reported
            return Task.CompletedTask;
        }
    }
}