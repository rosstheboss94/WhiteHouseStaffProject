using System.Data.SqlClient;
using System.Data;
using WhiteHouseETL.Helpers;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Tasks;

public class TaskSalary
{
    public static (List<Salary>, List<WhiteHouseStaff>) Transform(List<WhiteHouseStaff> records, List<WhiteHouseStaff> invalidRecords)
    {
        List<Salary> salaries = new List<Salary>();

        foreach (var record in records)
        {        
            if (ValidationHelpers.SalaryTableValidation(record.Salary, record.Year))
            {
                Salary salary = new Salary() { RowNumber = record.RowNumber, EmployeeSalary = record.Salary, Year = record.Year};
                salaries.Add(salary);
            }
            else
            {
                invalidRecords.Add(record);
            }                       
        }

        return (salaries, invalidRecords);
    }

    public static void Load(List<Salary> salaries)
    {
        using (SqlConnection connection = new SqlConnection("Data Source=TJ;Database=WhiteHouseETL;Integrated Security=True;"))
        {
            var ado = new ADOHelpers(connection);
            connection.Open();


            using (SqlCommand command = new SqlCommand("InsertSalaries", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = command.Parameters.AddWithValue("@Salaries", ado.CreateSalaryTable(salaries));
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = "SalaryTableType";

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
