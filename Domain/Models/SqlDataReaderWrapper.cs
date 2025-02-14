using Microsoft.Data.SqlClient;
using SQLReplicator.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Models
{
    public class SqlDataReaderWrapper : IDataReaderWrapper
    {
        private SqlDataReader _reader;

        public SqlDataReaderWrapper(SqlDataReader reader)
        {
            _reader = reader;
        }

        public List<string> ReadAttributes()
        {
            List<string> attributes = new List<string>();

            for (int i = 0; i < _reader.FieldCount; ++i)
            {
                attributes.Add(_reader.GetName(i));
            }

            return attributes;
        }

        public List<string> ReadValues()
        {
            List<string> values = new List<string>();

            for (int i = 0; i < _reader.FieldCount; ++i)
            {
                if (_reader.IsDBNull(i))
                {
                    values.Add("");
                }
                else
                {
                    values.Add(_reader.GetValue(i).ToString());
                }
            }

            return values;
        }
    }
}
