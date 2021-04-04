using AutoFixture;
using LispParser.Contracts;
using LispParser.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LispParser.Test.ServiceTests
{
    public class RunServiceTests
    {
        private RunService _sut;
        private Mock<IFileService> _fileServiceMock;
        private Mock<IParserService> _parserServiceMock;
        private Mock<IUserFeedbackService> _userFeedbackServiceMock;
        private IFixture fixture;

        [SetUp]
        public void SetUp()
        {
            _fileServiceMock = new Mock<IFileService>();
            _parserServiceMock = new Mock<IParserService>();
            _userFeedbackServiceMock = new Mock<IUserFeedbackService>();

            _sut = new RunService(
                _fileServiceMock.Object,
                _parserServiceMock.Object,
                _userFeedbackServiceMock.Object);

            fixture = new Fixture();
        }

        [Test]
        public void GetInputFilePath_ReturnsInput_WhenNotNullOrEmtpyAndFileExists()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            _fileServiceMock.Setup(s => s.FileExists(It.IsAny<string>())).Returns(true);

            // Act
            var response = _sut.GetInputFilePath(filePath);

            // Assert
            Assert.AreEqual(filePath, response);
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Never);
        }

        [Test]
        public void GetInputFilePath_ReturnsUserInput_WhenFileNotExists()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            var userInput = fixture.Create<string>();
            _fileServiceMock.Setup(s => s.FileExists(It.IsAny<string>())).Returns(false);
            _userFeedbackServiceMock.Setup(s => s.GetInputFileLocation()).Returns(userInput);

            // Act
            var response = _sut.GetInputFilePath(filePath);

            // Assert
            Assert.AreEqual(userInput, response);
            _fileServiceMock.Verify(s => s.FileExists(It.Is<string>(y => y == filePath)), Times.Once);
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Once);
        }

        [Test]
        public void GetInputFilePath_ReturnsUserInput_WhenNullOrEmtpyAndFileExists()
        {
            // Arrange
            var userInput = fixture.Create<string>();
            _userFeedbackServiceMock.Setup(s => s.GetInputFileLocation()).Returns(userInput);

            // Act
            var response = _sut.GetInputFilePath(string.Empty);

            //
            Assert.AreEqual(userInput, response);
            _userFeedbackServiceMock.Verify(s => s.GetInputFileLocation(), Times.Once);
        }
        [Test]
        public void GetFileContents_ThrowsExcption_WhenFileContentsEmpty()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            var fileContents = string.Empty;
            var expectedMessage = "LISP file cannot be empty.";

            _fileServiceMock.Setup(s => s.GetContents(It.IsAny<string>())).ReturnsAsync(fileContents);

            // Act
            // Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _sut.GetFileContents(filePath));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public async Task GetFileContents_ReturnsFileContents_WhenNotEmpty()
        {
            // Arrange
            var filePath = fixture.Create<string>();
            var fileContents = fixture.Create<string>();

            _fileServiceMock.Setup(s => s.GetContents(It.IsAny<string>())).ReturnsAsync(fileContents);

            // Act
            var result = await _sut.GetFileContents(filePath);

            // Assert
            _fileServiceMock.Verify(s => s.GetContents(It.Is<string>(x => x == filePath)), Times.Once);
            Assert.AreEqual(fileContents, result);
        }

        [Test]
        public void CleanContents_CallsAppropriateMethods_ReturnsFinalMethodResponse()
        {
            // Arrange
            var contents1 = fixture.Create<string>();
            var contents2 = fixture.Create<string>();
            var contents3 = fixture.Create<string>();

            _parserServiceMock.Setup(s => s.RemoveComments(It.IsAny<string>())).Returns(contents2);
            _parserServiceMock.Setup(s => s.RemoveQuotes(It.IsAny<string>())).Returns(contents3);

            // Act
            var response = _sut.CleanContents(contents1);

            // Assert
            _parserServiceMock.Verify(s => s.RemoveComments(It.Is<string>(x => x == contents1)), Times.Once);
            _parserServiceMock.Verify(s => s.RemoveQuotes(It.Is<string>(x => x == contents2)), Times.Once);

            Assert.AreEqual(contents3, response);
        }

        [Test]
        public void ValidateContents_CallsAppropriateMethods()
        {
            // Arrange
            var contents = fixture.Create<string>();

            // Act
            _sut.ValidateContents(contents);

            // Assert
            _parserServiceMock.Verify(s => s.ValidateCount(It.Is<string>(x => x == contents)), Times.Once);
            _parserServiceMock.Verify(s => s.ValidateNesting(It.Is<string>(x => x == contents)), Times.Once);
        }
    }
}
