using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using WhiteHouseETL.Helpers;
using WhiteHouseETL.Models;
using System.Formats.Asn1;
using System.Text.RegularExpressions;
using WhiteHouseETL.Tasks;

string filePath = @"C:\Users\torra\Desktop\Projects\WhiteHouseETL\WhiteHouseETL\BS&A whstafferdata.csv";
string fileName = Path.GetFileNameWithoutExtension(filePath);

using (var streamReader = new StreamReader(filePath))
{
    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
    {
        List<Employee> employees = new List<Employee>();
        List<WhiteHouseStaff> invalidRecords = new List<WhiteHouseStaff>();
        List<Position> positions = new List<Position>();

        csvReader.Context.RegisterClassMap<WhiteHouseStaffMap>();
        var records = csvReader.GetRecords<WhiteHouseStaff>().ToList();

        (records, invalidRecords) = ValidationHelpers.WhiteHouseStaffFileValidation(records);

        //(employees, invalidRecords) = TaskEmployee.Transform(records, invalidRecords);
        //TaskEmployee.Load(employees);

        (positions, invalidRecords) = TaskPosition.Transform(records, invalidRecords);
        TaskPosition.Load(positions);



        PrintInvalid(invalidRecords);
        Console.WriteLine("finished");
    }
}

Console.ReadKey();

void PrintInvalid(List<WhiteHouseStaff> invalidRecords)
{
    //Console.Write("year | name | gender | status | salary | pay_basis | position_title");
    //foreach (var record in invalidRecords)
    //{
    //    Console.WriteLine($"{record.Year} | {record.Name} | {record.Gender} | {record.Status} | {record.Salary} | {record.PayBasis} | {record.PositionTitle}");
    //}

    //var H = TransformationHelpers.SplitPositionTitle("WEST WING RECEPTIONIST AND COORDINATOR FOR OPERATIONS");
    //Console.WriteLine(ValidationHelpers.PositionTableValidation("ATTORNEY-ADVISOR", "", ""));
    // Console.WriteLine(Char.GetUnicodeCategory('-'));
    Console.WriteLine(invalidRecords.Count());
    
}
