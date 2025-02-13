using Serilog;
using SQLReplicator.Services.LoggerServices;
using System.Reflection.Metadata.Ecma335;

namespace SQLReplicator.Application
{
    public class Program
    {
        static void Main(string[] args)
        {
            LoggerService.InitializeLogger();

            

            LoggerService.CloseLogger();
        }
    }
}
