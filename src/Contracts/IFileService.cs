using System.Threading.Tasks;

namespace LispParser.Contracts
{
    public interface IFileService
    {
        bool FileExists(string filePath);
        Task<string> GetContents(string filePath);
    }
}
