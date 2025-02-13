using Serilog;
using SQLReplicator.Services.FileServices;
using SQLReplicator.Services.LoggerServices;
using System.Reflection.Metadata.Ecma335;

namespace SQLReplicator.Application
{
    public class Program
    {
        static void Main(string[] args)
        {
            LoggerService.InitializeLogger();
            Log.Information("Application has started.");

            #region ValidatingApplicationArguments
            if (args.Length != 1)
            {
                Log.Error("Wrong number of application arguments. Expected: filePath");
                return;
            }
            string filePath = args[0];
            #endregion

            #region ValidatingFileData
            List<string> dataFromFile = FileImportService.LoadData(filePath);

            if (dataFromFile.Count < 4)
            {
                Log.Error("File doesn't include necessary data. Expected: Two SQL connection strings, Last change version (default 0), Table name");
                return;
            }

            string connectionToServer1 = dataFromFile[0];
            string connectionToServer2 = dataFromFile[1];
            string lastChangeVersion = dataFromFile[2];
            string tableName = dataFromFile[3];
            #endregion


            Log.Information("Application has finished replication.");
            LoggerService.CloseLogger();
        }
    }
}
