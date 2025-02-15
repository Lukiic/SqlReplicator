using SQLReplicator.Domain.Interfaces;
using SQLReplicator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Services
{
    public interface IChangeTrackingDataService
    {
        public IDataReaderWrapper LoadData(string tableName);

        public void DeleteData(string tableName);
    }
}
