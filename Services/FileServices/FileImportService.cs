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
            catch (Exception)
            {
                importedLines = new List<string>();
            }

            return importedLines;
        }
    }
}
