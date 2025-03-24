using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Models
{
    public class DbTransactionManager : IDisposable
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;
        bool _isCommited;

        public DbTransactionManager(SqlConnection connection)
        {
            _connection = connection;

            _transaction = connection.BeginTransaction();
            _isCommited = false;
        }

        public SqlTransaction Transaction { get => _transaction; }

        public void Commit()
        {
            _transaction.Commit();
            _isCommited = true;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            if (!_isCommited)
            {
                Rollback();
            }

            _transaction.Dispose();
        }
    }
}
