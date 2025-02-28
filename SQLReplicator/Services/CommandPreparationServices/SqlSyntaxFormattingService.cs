namespace SQLReplicator.Services.CommandPreparationServices
{
    public class SqlSyntaxFormattingService
    {
        public static string GetInsertCommand(string tableName, List<string> attributes, List<string?> values)
        {
            values = RemoveExtraValues(attributes, values);

            string attributesFormat = string.Join(", ", attributes);

            string valuesFormat = string.Join(", ", values.Select(v => v == null ? "NULL" : $"'{v}'"));

            return $"INSERT INTO {tableName} ({attributesFormat}) VALUES ({valuesFormat});";
        }

        public static string GetDeleteCommand(string tableName, List<string> attributes, List<string?> values)
        {
            values = RemoveExtraValues(attributes, values);

            IEnumerable<string> attrsAssignValsFormat = attributes.Zip(values, (a, v) => v == null ? $"{a} IS NULL" : $"{a} = '{v}'");
            string conditionFormat = string.Join(" AND ", attrsAssignValsFormat);

            return $"DELETE FROM {tableName} WHERE {conditionFormat};";
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
