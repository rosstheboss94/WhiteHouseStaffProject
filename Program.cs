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
        List<ValidationResult> validationResults = new List<ValidationResult>();

        csvReader.Context.RegisterClassMap<WhiteHouseStaffMap>();
        var records = csvReader.GetRecords<WhiteHouseStaff>().ToList();

        (records, validationResults) = ValidationHelpers.WhiteHouseStaffFileValidation(records);

        (employees, validationResults) = TaskEmployee.Transform(records, validationResults);
        //TaskEmployee.Load(employees);

        (positions, validationResults) = TaskPosition.Transform(records, validationResults);
        //TaskPosition.Load(positions);

        (salaries, validationResults) = TaskSalary.Transform(records, validationResults);
        //TaskSalary.Load(salaries);

        PrintInvalid(validationResults);
        Console.WriteLine("finished");
    }

}

Console.ReadKey();

void PrintInvalid(List<ValidationResult> invalidRecords)
{
    //Console.Write("year | name | gender | status | salary | pay_basis | position_title");
    //foreach (var record in invalidRecords)
    //{
    //    Console.WriteLine($"{record.Year} | {record.Name} | {record.Gender} | {record.Status} | {record.Salary} | {record.PayBasis} | {record.PositionTitle}");
    //}

    Console.WriteLine(invalidRecords.Count());
    
}
