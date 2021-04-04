using AutoFixture;
using Moq;
using NUnit.Framework;

using LispParser.Contracts;
using LispParser.Services;

namespace LispParser.Test.ServiceTests
{
    public class UserFeedbackServiceTests
    {
        private UserFeedbackService _sut;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IConsole> _consoleMock;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _fileServiceMock = new Mock<IFileService>();
            _consoleMock = new Mock<IConsole>();
            _sut = new UserFeedbackService(_fileServiceMock.Object, _consoleMock.Object);
            fixture = new Fixture();
        }

        [Test]
        public void GetInputFileLocation_CallsWriteTextForUserOnce_WhenFileFromUserIntputExists()
        {
            // Arrange
            var consoleOutput = "Please type the location of the file you'd like to parse.";
            var userInput = fixture.Create<string>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns(userInput);
            _fileServiceMock.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            _sut.GetInputFileLocation();

            // Assert
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(o => o == false)), Times.Once);
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput)), Times.Once);
            _fileServiceMock.Verify(f => f.FileExists(It.Is<string>(p => p == userInput)), Times.Once);
        }

        [Test]
        public void GetInputFileLocation_CallsWriteTextForUserTwice_WhenFileFromUserIntputNotExists()
        {
            // Arrange
            var consoleOutput1 = "Please type the location of the file you'd like to parse.";
            var consoleOutput2 = "Please select a valid file. File does not exist.";
            var userInput1 = fixture.Create<string>();
            var userInput2 = fixture.Create<string>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == false))).Returns(userInput1);
            _consoleMock.Setup(c => c.GetTextFromUser(It.Is<bool>(x => x == true))).Returns(userInput2);
            _fileServiceMock.Setup(f => f.FileExists(It.Is<string>(p => p == userInput1))).Returns(false);
            _fileServiceMock.Setup(f => f.FileExists(It.Is<string>(p => p == userInput2))).Returns(true);

            // Act
            _sut.GetInputFileLocation();

            // Assert
            _consoleMock.Verify(c => c.GetTextFromUser(It.Is<bool>(o => o == false)), Times.Once);
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput1)), Times.Exactly(2));
            _consoleMock.Verify(c => c.WriteTextForUser(It.Is<string>(o => o == consoleOutput2)), Times.Once);
            _fileServiceMock.Verify(f => f.FileExists(It.Is<string>(p => p == userInput1)), Times.Once);
            _fileServiceMock.Verify(f => f.FileExists(It.Is<string>(p => p == userInput2)), Times.Once);
        }

        [Test]
        public void GetInputFileLocation_ReturnsUserInput()
        {
            // Arrange
            var userInput = fixture.Create<string>();

            _consoleMock.Setup(c => c.GetTextFromUser(It.IsAny<bool>())).Returns(userInput);
            _fileServiceMock.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            var result = _sut.GetInputFileLocation();

            // Assert
            Assert.AreEqual(userInput, result);
        }
    }
}
