using Serilog;

namespace SQLReplicator.Services.FileServices
{
    public class FileImportService
    {
        public static List<string> LoadData(string path)
        {
            ValidateArguments(path);

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

        private static void ValidateArguments(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("File path cannot be null, empty, or whitespace.", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file \"{path}\" does not exist.", path);
            }
        }
    }
}
