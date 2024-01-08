using System.Data.SqlClient;
using System.Data;
using WhiteHouseETL.Helpers;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Tasks;

public static class TaskEmployee
{
    public static (List<Employee>, List<WhiteHouseStaff>) Transform(List<WhiteHouseStaff> records, List<WhiteHouseStaff> invalidRecords)
    {
        List<Employee> employees = new List<Employee>();

        foreach (var record in records)
        {
            if (ValidationHelpers.EmployeeTableValidation(record.Name))
            {
                Employee employee = new Employee()
                {
                    RowNumber = record.RowNumber,
                    FirstName = TransformationHelpers.ExtractNameWithDeLimiters(record.Name, type: "BETWEEN", delimiterLeft: ",", delimiterRight: " "),
                    MiddleInitial = TransformationHelpers.ExtractNameWithDeLimiters(record.Name, type: "RIGHT", delimiterRight: " "),
                    LastName = TransformationHelpers.ExtractNameWithDeLimiters(record.Name, delimiterLeft: ","),
                    Gender = record.Gender,
                };

                employees.Add(employee);
            }
            else
            {
                WhiteHouseStaff invalid = new WhiteHouseStaff()
                {
                    Year = record.Year,
                    Name = record.Name,
                    Gender = record.Gender,
                    Status = record.Status,
                    Salary = record.Salary,
                    PayBasis = record.PayBasis,
                    PositionTitle = record.PositionTitle
                };

                invalidRecords.Add(invalid);
            }
        }

        return (employees, invalidRecords);
    }

    public static void Load(List<Employee> employees)
    {
        using (SqlConnection connection = new SqlConnection("Data Source=TJ;Database=WhiteHouseETL;Integrated Security=True;"))
        {
            var ado = new ADOHelpers(connection);
            connection.Open();


            using (SqlCommand command = new SqlCommand("InsertEmployees", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = command.Parameters.AddWithValue("@Employees", ado.CreateEmployeeTable(employees));
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = "EmployeeTableType";

                command.ExecuteNonQuery();
            }

            connection.Close();

        }
    }
}
