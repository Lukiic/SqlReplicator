using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.FileServices
{
    public class FileExportService
    {
        public static void SaveToFile(string filePath, List<string> data)
        {
			try
			{
				File.WriteAllLines(filePath, data);
				Log.Information($"Saved data to file {filePath}");
			}
			catch (Exception ex)
			{
				Log.Warning($"Failed to save data to file {filePath}");
			}
        }
    }
}
