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
            foreach (string command in commands)
            {
                _executeSqlCommandService.ExecuteCommand(command);
            }
        }
    }
}
