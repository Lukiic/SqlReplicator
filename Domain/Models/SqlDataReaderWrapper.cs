﻿using Microsoft.Data.SqlClient;
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

        public void Dispose()
        {
            _reader.Close();
            _reader.Dispose();
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

        public List<List<string>> ReadValues()      // Inner list represents one row of table
        {
            List<List<string>> allValues = new List<List<string>>();

            while (_reader.Read())
            {
                List<string> row = new List<string>();

                for (int i = 0; i < _reader.FieldCount; ++i)
                {
                    if (_reader.IsDBNull(i))
                    {
                        row.Add("");
                    }
                    else
                    {
                        row.Add(_reader.GetValue(i).ToString());
                    }
                }

                allValues.Add(row);
            }

            return allValues;
        }
    }
}
