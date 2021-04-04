using LispParser.Contracts;

namespace LispParser.Services
{
    public class UserFeedbackService : IUserFeedbackService
    {
        private readonly IFileService _fileService;
        private readonly IConsole _console;

        public UserFeedbackService(IFileService fileService, IConsole console)
        {
            _fileService = fileService;
            _console = console;
        }

        public string GetInputFileLocation()
        {
            var response = string.Empty;
            var hasFailed = false;

            while (string.IsNullOrEmpty(response))
            {
                _console.WriteTextForUser("Please type the location of the file you'd like to parse.");
                response = _console.GetTextFromUser(hasFailed);

                if (!_fileService.FileExists(response))
                {
                    _console.WriteTextForUser("Please select a valid file. File does not exist.");
                    response = string.Empty;
                    hasFailed = true;
                }
            }

            return response;
        }
    }
}
