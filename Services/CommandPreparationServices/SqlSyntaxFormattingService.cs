﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.CommandPreparationServices
{
    public class SqlSyntaxFormattingService
    {
        public static string GetInsertCommand(string tableName, List<string> attributes, List<string> values)
        {
            string attributesFormat = string.Join(", ", attributes);

            string valuesFormat = string.Join(", ", values.Select(v => $"'{v}'"));

            return $"INSERT INTO {tableName} ({attributesFormat}) VALUES ({valuesFormat});";
        }

        public static string GetUpdateCommand(string tableName, List<string> attributes, List<string> newValues, List<string> oldValues)
        {
            string setFormat = string.Join(", ", attributes.Zip(oldValues, (a, v) => $"{a} = '{v}'"));
            string conditionFormat = string.Join(" AND ", attributes.Zip(oldValues, (a, v) => $"{a} = '{v}'"));
            
            return $"UPDATE {tableName} SET {setFormat} WHERE {conditionFormat};";
        }

        public static string GetDeleteCommand(string tableName, List<string> attributes, List<string> values)
        {
            string conditionFormat = string.Join(" AND ", attributes.Zip(values, (a, v) => $"{a} = '{v}'"));

            return $"DELETE FROM {tableName} WHERE {conditionFormat};";
        }
    }
}
