using System.Data;
using System.Data.SqlClient;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Helpers;

public class ADOHelpers
{
    private SqlConnection _connection;
    public ADOHelpers(SqlConnection connection)
    {
        _connection = connection;
    }

    public void runNonQuery(string query)
    {
        using (SqlCommand command = new SqlCommand(query, _connection))
        {
            _connection.Open();
            Console.WriteLine("running query... \n" + query);
            command.ExecuteNonQuery();
            Console.WriteLine("query completed");
            _connection.Close();
        }
    }

    public List<Dictionary<string, string>> runStoredProcedure(string storedProcedure)
    {
        List<Dictionary<string, string>> tableRows = new List<Dictionary<string, string>>();

        _connection.Open();
        using (SqlCommand command = new SqlCommand(storedProcedure, _connection))
        {
            command.CommandType = CommandType.StoredProcedure;

            Console.WriteLine("Running procedure" + storedProcedure);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                List<string> columns = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetName(i));
                }

                while (reader.Read())
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(columns[i], reader.GetString(reader.GetOrdinal(columns[i])));
                    }

                    tableRows.Add(row);
                }
            }
        }

        _connection.Close();
        return tableRows;
    }

    public DataTable CreateEmployeeTable(List<Employee> employees)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Row Number", typeof(int));
        dt.Columns.Add("First Name", typeof(string));
        dt.Columns.Add("Middle Inital", typeof(string));
        dt.Columns.Add("Last Name", typeof(string));
        dt.Columns.Add("Gender", typeof(string));

        foreach (var employee in employees)
        {
            dt.Rows.Add(employee.RowNumber, employee.FirstName, employee.MiddleInitial, employee.LastName, employee.Gender);
        }

        return dt;
    }

    public DataTable CreatePositionTable(List<Position> positions)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Row Number", typeof(int));
        dt.Columns.Add("Position Title", typeof(string));
        dt.Columns.Add("Pay Basis", typeof(string));
        dt.Columns.Add("Status", typeof(string));

        foreach (var position in positions)
        {
            dt.Rows.Add(position.RowNumber, position.PositionTitle, position.PayBasis, position.Status);
        }

        return dt;
    }
}
