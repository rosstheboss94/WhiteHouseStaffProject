using System.Text;

namespace WhiteHouseETL.Helpers;

public static class SQLQueryHelpers
{
    public static string CreateTableName(string fileName)
    { 
        fileName = fileName.Replace(" ", "_");
        fileName = fileName.Replace("&", "_");
        return fileName;
    }

    public static string CreateTempTable(string[] headers, string fileName)
    {
        fileName = fileName.Replace(" ", "_");
        fileName = fileName.Replace("&", "_");

        string prefix =
            $"IF OBJECT_ID('{fileName}', 'U') IS NULL " +
            "BEGIN " +
            $"CREATE TABLE {fileName} (";

        string suffix = ")" + " END";
        string columns = "";


        for (int i = 0; i < headers.Length; i++)
        {
            if (i == headers.Length - 1)
            {
                columns = columns + headers[i] + " NVARCHAR(200)";
            }
            else
            {
                columns = columns + headers[i] + " NVARCHAR(200),";
            }
        }

        return prefix + columns + suffix;
    }

    public static string BulkInsertCSV(string filePath, string fileName, string fieldTerminator = ",", string fieldQuote = "\"", string rowTerminator = "\\n", string firstRow = "2")
    {
        string table = CreateTableName(fileName);
        StringBuilder bulkInsertSQL = new StringBuilder();

        bulkInsertSQL.AppendLine($"TRUNCATE TABLE {table}");
        bulkInsertSQL.AppendLine($"BULK INSERT {table}");
        bulkInsertSQL.AppendLine($"FROM '{filePath}'");
        bulkInsertSQL.AppendLine("WITH");
        bulkInsertSQL.AppendLine("(");
        bulkInsertSQL.AppendLine("FORMAT = 'CSV',");
        bulkInsertSQL.AppendLine($"FieldTerminator = '{fieldTerminator}',");
        bulkInsertSQL.AppendLine($"FieldQuote = '{fieldQuote}',");
        bulkInsertSQL.AppendLine($"RowTerminator = '{rowTerminator}',");
        bulkInsertSQL.AppendLine($"FirstRow = {firstRow}");
        bulkInsertSQL.AppendLine(")");

        return bulkInsertSQL.ToString();
    }

    public static bool ValidateTransformations(List<Dictionary<string,string>> result)
    {
        return true;
    }
}
