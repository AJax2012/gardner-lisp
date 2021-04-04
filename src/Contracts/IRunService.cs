using System.Threading.Tasks;

namespace LispParser.Contracts
{
    public interface IRunService
    {
        string CleanContents(string contents);
        Task<string> GetFileContents(string filePath);
        string GetInputFilePath(string filepath);
        void ValidateContents(string contents);
    }
}
