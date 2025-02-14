using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Services
{
    public interface ICreateChangeTrackingTableService
    {
        public bool CreateCTTable(string tableName);
    }
}
