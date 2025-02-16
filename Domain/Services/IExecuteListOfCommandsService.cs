using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Services
{
    public interface IExecuteListOfCommandsService
    {
        public void ExecuteCommands(List<string> commands);
    }
}
