using Serilog;

namespace SQLReplicator.Application
{
    public class AppTerminationHandler
    {
        // Handler for CTRL+C action to stop the main loop
        public static void SetupHandler(AppState appState)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                appState.ShouldRun = false;
                Log.Information("Termination signal received. Exiting main loop and preparing for shutdown.");
            };
        }
    }
}
