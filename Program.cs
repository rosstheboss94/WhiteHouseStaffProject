using CsvHelper;
using System.Globalization;
using WhiteHouseETL.Helpers;
using WhiteHouseETL.Models;
using WhiteHouseETL.Tasks;

string filePath = @"C:\Users\torra\Desktop\Projects\WhiteHouseETL\WhiteHouseETL\BS&A whstafferdata.csv";

using (var streamReader = new StreamReader(filePath))
{
    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
    {
        List<Employee> employees = new List<Employee>();
        List<Position> positions = new List<Position>();
        List<Salary> salaries = new List<Salary>();
        List<WhiteHouseStaff> invalidRecords = new List<WhiteHouseStaff>();

        csvReader.Context.RegisterClassMap<WhiteHouseStaffMap>();
        var records = csvReader.GetRecords<WhiteHouseStaff>().ToList();

        (records, invalidRecords) = ValidationHelpers.WhiteHouseStaffFileValidation(records);

        (employees, invalidRecords) = TaskEmployee.Transform(records, invalidRecords);
        TaskEmployee.Load(employees);

        (positions, invalidRecords) = TaskPosition.Transform(records, invalidRecords);
        TaskPosition.Load(positions);

        (salaries, invalidRecords) = TaskSalary.Transform(records, invalidRecords);
        TaskSalary.Load(salaries);

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

    Console.WriteLine(invalidRecords.Count());
    
}
