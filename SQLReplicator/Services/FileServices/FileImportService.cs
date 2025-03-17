using Serilog;

namespace SQLReplicator.Services.FileServices
{
    public class FileImportService
    {
        public static List<string> LoadData(string path)
        {
            List<string> importedLines;

            try
            {
                importedLines = File.ReadAllLines(path).ToList();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"Failed to load data from file \"{path}\".");
                importedLines = new List<string>();
            }

            return importedLines;
        }
    }
}
