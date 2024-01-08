using System.Data.SqlClient;
using System.Data;
using WhiteHouseETL.Helpers;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Tasks;

public class TaskPosition
{
    public static (List<Position>, List<ValidationResult>) Transform(List<WhiteHouseStaff> records, List<ValidationResult> validationResults)
    {
        List<Position> positions = new List<Position>();

        foreach (var record in records)
        {
 
            var roles = TransformationHelpers.SplitPositionTitle(record.PositionTitle);
            ValidationResult validationResult;

            if (roles.Length > 1)
            {
                foreach (var role in roles)
                {
                    validationResult = ValidationHelpers.PositionTableValidation(role, record.PayBasis, record.Status);
                    if (validationResult.Passed)
                    {
                        Position position = new Position() { RowNumber = record.RowNumber, PositionTitle = role.Trim(), PayBasis = record.PayBasis, Status = record.Status };
                        positions.Add(position);
                    }
                    else
                    {
                        validationResults.Add(validationResult);
                    }
                }
            }
            else
            {
                validationResult = ValidationHelpers.PositionTableValidation(roles[0], record.PayBasis, record.Status);
                if (validationResult.Passed)
                {
                    Position position = new Position() { RowNumber = record.RowNumber, PositionTitle = roles[0].Trim(), PayBasis = record.PayBasis, Status = record.Status };
                    positions.Add(position);
                }
                else
                {
                    validationResults.Add(validationResult);
                }
            }
        }

        return (positions, validationResults);
    }

    public static void Load(List<Position> positions)
    {
        using (SqlConnection connection = new SqlConnection("Data Source=TJ;Database=WhiteHouseETL;Integrated Security=True;"))
        {
            var ado = new ADOHelpers(connection);
            connection.Open();

            using (SqlCommand command = new SqlCommand("InsertPositions", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = command.Parameters.AddWithValue("@Positions", ado.CreatePositionTable(positions));
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = "PositionTableType";

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
