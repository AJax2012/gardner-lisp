using AutoFixture;
using LispParser.Contracts;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace LispParser.Test
{
    public class RunProgramTests
    {
        private RunProgram _sut;
        private Mock<IRunService> _runServiceMock;
        private IFixture fixture;

        [SetUp]
        public void Setup()
        {
            _runServiceMock = new Mock<IRunService>();
            _sut = new RunProgram(_runServiceMock.Object);
            fixture = new Fixture();
        }

        [Test]
        public async Task RunAsync_CallsAppropriateMethods()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            var fileContents = fixture.Create<string>();
            var parsedFileContents = fixture.Create<string>();

            _runServiceMock.Setup(s => s.GetInputFilePath(It.IsAny<string>())).Returns(filePath);
            _runServiceMock.Setup(s => s.GetFileContents(It.IsAny<string>())).ReturnsAsync(fileContents);
            _runServiceMock.Setup(s => s.CleanContents(It.IsAny<string>())).Returns(parsedFileContents);

            // Act
            await _sut.RunAsync(new string[] { }, new CancellationToken());

            // Assert
            _runServiceMock.Verify(s => s.GetInputFilePath(It.Is<string>(y => y == string.Empty)), Times.Once);
            _runServiceMock.Verify(s => s.GetFileContents(It.Is<string>(y => y == filePath)), Times.Once);
            _runServiceMock.Verify(s => s.CleanContents(It.Is<string>(y => y == fileContents)), Times.Once);
            _runServiceMock.Verify(s => s.ValidateContents(It.Is<string>(y => y == parsedFileContents)), Times.Once);
        }

        [Test]
        public async Task RunAsync_ParsesArgsCorrectly()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            var args = new[] { filePath };
            var fileContents = fixture.Create<string>();
            var parsedFileContents = fixture.Create<string>();

            _runServiceMock.Setup(s => s.GetInputFilePath(It.IsAny<string>())).Returns(filePath);
            _runServiceMock.Setup(s => s.GetFileContents(It.IsAny<string>())).ReturnsAsync(fileContents);
            _runServiceMock.Setup(s => s.CleanContents(It.IsAny<string>())).Returns(parsedFileContents);

            // Act
            await _sut.RunAsync(args, new CancellationToken());

            // Assert
            _runServiceMock.Verify(s => s.GetInputFilePath(It.Is<string>(y => y == filePath)), Times.Once);
        }
    }
}