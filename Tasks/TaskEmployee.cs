using System.Data.SqlClient;
using System.Data;
using WhiteHouseETL.Helpers;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Tasks;

public static class TaskEmployee
{
    public static (List<Employee>, List<ValidationResult>) Transform(List<WhiteHouseStaff> records, List<ValidationResult> validationResults)
    {
        List<Employee> employees = new List<Employee>();

        foreach (var record in records)
        {
            ValidationResult validationResult = ValidationHelpers.EmployeeTableValidation(record.Name);
            Employee employee = new Employee()
            {
                RowNumber = record.RowNumber,
                FirstName = TransformationHelpers.ExtractNameWithDeLimiters(record.Name, type: "BETWEEN", delimiterLeft: ",", delimiterRight: " "),
                MiddleInitial = TransformationHelpers.ExtractNameWithDeLimiters(record.Name, type: "RIGHT", delimiterRight: " "),
                LastName = TransformationHelpers.ExtractNameWithDeLimiters(record.Name, delimiterLeft: ","),
                Gender = record.Gender,
            };

            if (validationResult.Passed)
            {
                employees.Add(employee);
            }
            else
            {
                validationResult.Employee = employee;
                validationResult.Record = record;
                validationResults.Add(validationResult);
            }
        }

        return (employees, validationResults);
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
