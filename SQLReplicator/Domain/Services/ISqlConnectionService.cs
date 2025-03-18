using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Services
{
    public interface ISqlConnectionService
    {
        public void OpenConnection(SqlConnection sqlConnection);

        public void CloseConnection(SqlConnection sqlConnection);
    }
}
