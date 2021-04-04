using LispParser.Contracts;
using System;
using System.Threading.Tasks;

namespace LispParser.Services
{
    public class RunService : IRunService
    {
        private readonly IFileService _fileService;
        private readonly IParserService _parserService;
        private readonly IUserFeedbackService _userFeedbackService;

        public RunService(IFileService fileService, IParserService parserService, IUserFeedbackService userFeedbackService)
        {
            _fileService = fileService;
            _parserService = parserService;
            _userFeedbackService = userFeedbackService;
        }

        public string GetInputFilePath(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && _fileService.FileExists(filePath))
            {
                return filePath;
            }

            return _userFeedbackService.GetInputFileLocation();
        }

        public async Task<string> GetFileContents(string filePath)
        {
            var fileContents = await _fileService.GetContents(filePath);

            if (string.IsNullOrWhiteSpace(fileContents))
            {
                throw new ArgumentException("LISP file cannot be empty.");
            }

            return fileContents;
        }

        public string CleanContents(string contents)
        {
            contents = _parserService.RemoveComments(contents);
            return _parserService.RemoveQuotes(contents);
        }

        public void ValidateContents(string contents)
        {
            _parserService.ValidateCount(contents);
            _parserService.ValidateNesting(contents);
        }
    }
}
