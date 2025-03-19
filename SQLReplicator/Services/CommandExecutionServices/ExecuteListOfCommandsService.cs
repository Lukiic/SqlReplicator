using Microsoft.Data.SqlClient;
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

        // Returns the count of commands that were not executed from the input list
        public int ExecuteCommands(List<string> commands)
        {
            ValidateArguments(commands);
            if (commands.Count == 0)
            {
                return 0;
            }

            Log.Information($"Execution of {commands.Count} SQL commands started.");

            for (int i = 0; i < commands.Count; ++i)
            {
                string command = commands[i];
                try
                {
                    _executeSqlCommandService.ExecuteCommand(command);
                }
                catch (SqlException ex) when (ex.Number == 515) // NOT NULL constraint violation is normal for this application, so those commands are ignored
                {
                    continue;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, $"Failed to execute command: {command}");
                    return commands.Count - i;
                }
            }

            Log.Information($"Execution of {commands.Count} SQL commands finished successfully.");

            return 0;   // No commands were left unexecuted
        }

        private void ValidateArguments(List<string> commands)
        {
            if (commands == null)
            {
                throw new ArgumentException("Commands list cannot be null.", nameof(commands));
            }

            if (commands.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Commands list cannot contain null or empty commands.", nameof(commands));
            }
        }
    }
}
