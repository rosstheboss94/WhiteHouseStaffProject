﻿using CsvHelper;
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
        TaskEmployee.Load(employees);

        (positions, validationResults) = TaskPosition.Transform(records, validationResults);
        TaskPosition.Load(positions);

        (salaries, validationResults) = TaskSalary.Transform(records, validationResults);
        TaskSalary.Load(salaries);

        PrintInvalid(validationResults);
    }
}

//Console.ReadKey();

void PrintInvalid(List<ValidationResult> invalidRecords)
{
    string content = "";

    foreach (var record in invalidRecords)
    {
        string yearStr = record.Record.Year == 1900 ? "" : record.Record.Year.ToString();
        string salaryStr = record.Record.Salary == 0 ? "" : record.Record.Salary.ToString();

        content = content + "Record: " + $"\n\tYear - {yearStr} \n\tName - {record.Record.Name} \n\tGender - {record.Record.Gender} \n\tStatus - {record.Record.Status} \n\tSalary - {salaryStr} \n\tPay Basis - {record.Record.PayBasis} \n\tPosition Title - {record.Record.PositionTitle}" + "\n";

        if (record.Results["Validation Type"] == "Employee") content = content + "Employee: " + $"\n\tFirst Name - {record.Employee.FirstName} \n\tMiddle Initial - {record.Employee.MiddleInitial} \n\tLast Name - {record.Employee.LastName} \n\tGender - {record.Employee.Gender} \n";
        if (record.Results["Validation Type"] == "Position") content = content + "Position: " + $"\n\tPosition Title - {record.Position.PositionTitle} \n\tPay Basis - {record.Position.PayBasis} \n\tStatus - {record.Position.Status} \n";
        if (record.Results["Validation Type"] == "Salary") 
        {
            string salaryYearStr = record.Salary.Year == 1900 ? "" : record.Salary.Year.ToString();
            string salarySalaryStr = record.Salary.EmployeeSalary == 0 ? "" : record.Salary.EmployeeSalary.ToString();
            content = content + "Salary: " + $"\n\tSalary - {salarySalaryStr} \n\tYear - {salaryYearStr} \n";
        } 

        content = content + "Outcome: " + record.Passed + "\n";
        content = content + "Results: \n";

        foreach (KeyValuePair<string, string> kvp in record.Results)
        {
            content = content + "\t" + kvp.Key + ": " + kvp.Value + "\n";
        }

        content = content + "\n";
    }

    File.WriteAllText(@"C:\Users\torra\Desktop\Projects\WhiteHouseETL\WhiteHouseETL\ValidationResult.txt", content);

    Console.WriteLine(invalidRecords.Count());
    Console.WriteLine("finished");
}
