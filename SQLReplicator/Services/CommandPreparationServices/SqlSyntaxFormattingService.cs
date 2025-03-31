namespace SQLReplicator.Services.CommandPreparationServices
{
    public class SqlSyntaxFormattingService
    {
        public static string GetInsertCommand(string tableName, List<string> attributes, List<string?> values)
        {
            ValidateArguments(tableName, attributes, values);

            values = RemoveExtraValues(attributes, values);

            string attributesFormat = string.Join(", ", attributes);

            string valuesFormat = string.Join(", ", values.Select(v => v == null ? "NULL" : $"'{v}'"));

            return $"INSERT INTO {tableName} ({attributesFormat}) VALUES ({valuesFormat});";
        }

        public static string GetDeleteCommand(string tableName, List<string> attributes, List<string?> values)
        {
            ValidateArguments(tableName, attributes, values);

            values = RemoveExtraValues(attributes, values);

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(values, (a, v) => v == null ? $"{a} IS NULL" : $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            return $"DELETE FROM {tableName} WHERE {conditionFormat};";
        }

        public static string GetUpdateCommand(string tableName, List<string> keyAttrs, List<string?> keyValues, List<string> attributes, List<string?> values)
        {
            ValidateArguments(tableName, attributes, values);
            ValidateArguments(tableName, keyAttrs, keyValues);

            values = RemoveExtraValues(attributes, values);
            keyValues = RemoveExtraValues(keyAttrs, keyValues);

            IEnumerable<string> keyAttrsAssignValsFormat = keyAttrs.Zip(keyValues, (a, v) => $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", keyAttrsAssignValsFormat);

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(values, (a, v) => v == null ? $"{a} = NULL" : $"{a} = '{v}'");
            string setFormat = string.Join(", ", attrsAssignValsFormat);

            return $"UPDATE {tableName} SET {setFormat} WHERE {conditionFormat};";
        }

        private static void ValidateArguments(string tableName, List<string> attributes, List<string?> values)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("Table name cannot be null, empty, or whitespace.", nameof(tableName));
            }

            if (attributes == null || attributes.Count == 0)
            {
                throw new ArgumentException("Attributes cannot be null or empty.", nameof(attributes));
            }

            if (values == null || values.Count == 0)
            {
                throw new ArgumentException("Values cannot be null or empty.", nameof(values));
            }
        }

        private static List<string?> RemoveExtraValues(List<string> attributes, List<string?> values)
        {
            if (attributes.Count == values.Count)
            {
                return values;
            }

            return values.GetRange(0, attributes.Count);
        }
    }
}
