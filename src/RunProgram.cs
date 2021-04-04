using System.Threading;
using System.Threading.Tasks;

using LispParser.Contracts;

namespace LispParser
{
    public class RunProgram
    {
        private readonly IRunService _runService;

        public RunProgram(IRunService runService)
        {
            _runService = runService;
        }

        public async Task RunAsync(string[] args, CancellationToken cancellationToken)
        {
            var filePath = string.Empty;

            if (args.Length > 0)
            {
                filePath = args[0];
            }

            filePath = _runService.GetInputFilePath(filePath);
            var fileContents = await _runService.GetFileContents(filePath);
            fileContents = _runService.CleanContents(fileContents);
            _runService.ValidateContents(fileContents);
        }
    }
}
