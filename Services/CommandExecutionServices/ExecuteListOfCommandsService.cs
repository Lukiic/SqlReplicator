using Serilog;
using SQLReplicator.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int i = 0;

            foreach (string command in commands)
            {
                try
                {
                    _executeSqlCommandService.ExecuteCommand(command);
                    Log.Information($"Executed command number: {++i}");
                }
                catch (Exception)
                {
                    Log.Warning($"Failed to execute command: {command}");
                }
            }
        }
    }
}
