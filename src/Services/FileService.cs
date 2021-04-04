using System.IO;
using System.Threading.Tasks;

using LispParser.Contracts;

namespace LispParser.Services
{
    public class FileService : IFileService
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public async Task<string> GetContents(string filePath)
        {
            var records = string.Empty;

            using (var stream = new StreamReader(filePath))
            {
                records = await stream.ReadToEndAsync();
            }

            return records;
        }
    }
}
