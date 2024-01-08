using CsvHelper.Configuration;

namespace WhiteHouseETL.Models;

public class Position
{
    public int RowNumber { get; set; }
    public string PositionTitle { get; set; }
    public string PayBasis { get; set; }
    public string Status { get; set; }
}

public class PositionMap : ClassMap<Position>
{
    public PositionMap()
    {
        Map(m => m.PositionTitle).Name("position_title").Default("");
        Map(m => m.PayBasis).Name("pay_basis").Default("");
        Map(m => m.Status).Name("status").Default("");
    }
}
