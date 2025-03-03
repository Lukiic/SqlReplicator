using Serilog;
using SQLReplicator.Domain.Services;

namespace SQLReplicator.Services.CommandExecutionServices
{
    public class ExecuteListOfCommandsService : IExecuteListOfCommandsService
    {
        private IExecuteSqlCommandService _executeSqlCommandService;

        public ExecuteListOfCommandsService(IExecuteSqlCommandService executeSqlCommandService)
        {
            _executeSqlCommandService = executeSqlCommandService;
        }

        public void ExecuteCommands(List<string> commands)
        {
            if (commands.Count == 0)
            {
                return;
            }

            Log.Information($"Execution of {commands.Count} SQL commands started.");
            int i = 0;

            foreach (string command in commands)
            {
                try
                {
                    _executeSqlCommandService.ExecuteCommand(command);

                    if (++i % 1000 == 0)
                    {
                        Log.Debug($"Executed {i} commands out of {commands.Count} so far.");
                    }
                }
                catch (Exception)
                {
                    Log.Warning($"Failed to execute command: {command}");
                }
            }

            Log.Information($"Successfully executed {i} out of {commands.Count} commands.");
        }
    }
}
