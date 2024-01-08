using System.ComponentModel;
using System.Text.RegularExpressions;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Helpers;

public static class ValidationHelpers
{
    public static ValidationResult EmployeeTableValidation(string name)
    {
        ValidationResult validationResult = new ValidationResult() { Passed = false};

        if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\.,[A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\. ")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+ [A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.+ [A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.+ [A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+ [A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.,[A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+,[A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+ [A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.,[A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+,[A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]")) validationResult.Passed = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]")) validationResult.Passed = true;

        return validationResult;
    }

    public static ValidationResult PositionTableValidation(string position, string payBasis, string status)
    {
        ValidationResult validationResult = new ValidationResult();
        bool positionValid = false;
        bool payBasisValid = false;
        bool statusValid = false;

        int invalidCharactersCount = 0;
        string invalidCharacters = "";
        int ofIndex = position.IndexOf(" OF ");
        int forIndex = position.IndexOf(" FOR ");
        int toIndex = position.IndexOf(" TO ");
        int andIndex = position.IndexOf(" AND ");

        foreach (char ch in position)
        {
            if (!(Char.IsLetter(ch) || ch == ' ' || ch == ',' || ch == '\''))
            {
                invalidCharactersCount++;
                invalidCharacters = invalidCharacters + ch + " ";
            }
        }

        if (andIndex >= 0) 
        {
            if (ofIndex == -1 && forIndex == -1 && toIndex == -1) positionValid = true;

            if (andIndex > ofIndex || andIndex > forIndex || andIndex > toIndex) positionValid = true;
        }
        else
        {
            positionValid = true;
        }

        if (payBasis == ValidationHelpers.GetEnumDescription(PayBasisEnum.PayBasis.PerDiem)
            || payBasis == ValidationHelpers.GetEnumDescription(PayBasisEnum.PayBasis.PerAnnum)) payBasisValid = true;

        if (status == ValidationHelpers.GetEnumDescription(StatusEnum.Status.Employee)
            || status == ValidationHelpers.GetEnumDescription(StatusEnum.Status.Detailee)
            || status == ValidationHelpers.GetEnumDescription(StatusEnum.Status.PartTime)) statusValid = true;

        
        if (positionValid == true && payBasisValid == true && statusValid == true && invalidCharactersCount == 0 && !ValidationHelpers.ContainsSeparator(position))
        {
            validationResult.Passed = true;
            validationResult.Results.Add("Invalid Characters:", invalidCharacters);
            validationResult.Results.Add("Position Valid:", positionValid.ToString());
            validationResult.Results.Add("Pay Basis Valid:", payBasisValid.ToString());
            validationResult.Results.Add("Status Valid:", statusValid.ToString());
            return validationResult;
        }
        else
        {
            validationResult.Passed = false;
            validationResult.Results.Add("Invalid Characters:", invalidCharacters);
            validationResult.Results.Add("Position Valid:", positionValid.ToString());
            validationResult.Results.Add("Pay Basis Valid:", payBasisValid.ToString());
            validationResult.Results.Add("Status Valid:", statusValid.ToString());
            return validationResult;
        }
    }

    public static (List<WhiteHouseStaff>, List<ValidationResult>) WhiteHouseStaffFileValidation(List<WhiteHouseStaff> records) 
    {
        List<WhiteHouseStaff> validRecords = new List<WhiteHouseStaff>();
        List<ValidationResult> validationResults = new List<ValidationResult>();

        foreach (WhiteHouseStaff record in records)
        {
            if(record.Year != 1900 || record.Name != "" || record.Gender != "" || record.Status != "" || record.Salary != 0 || record.PayBasis != "" || record.PositionTitle != "")
            {
                validRecords.Add(new WhiteHouseStaff()
                {
                    RowNumber = record.RowNumber,
                    Year = record.Year,
                    Name = record.Name,
                    Gender = record.Gender,
                    Salary = record.Salary,
                    Status = record.Status,
                    PayBasis = record.PayBasis,
                    PositionTitle = record.PositionTitle
                });
            }
            else
            {
                validationResults.Add(new ValidationResult
                {
                    Record = record,
                    Passed = false,
                    Results = new Dictionary<string, string>{ { "Contains missing values", "true" } }
                });
            }
        }

        return (validRecords, validationResults);
    }

    public static ValidationResult SalaryTableValidation(int salary, int year)
    {
        ValidationResult validationResult = new ValidationResult();
        bool salaryValid = salary > 0 ? true : false;
        bool yearValid = year > 1900 ? true : false;

        if (salaryValid && yearValid)
        {
            validationResult.Passed = true;
        }
        else
        {
            validationResult.Passed = false;
            validationResult.Salary = new Salary() { EmployeeSalary = salary, Year = year };
            validationResult.Results = new Dictionary<string, string>() { { "Salary value valid: ", salaryValid.ToString() }, { "Year value valid: ", yearValid.ToString() } };
        }

        return validationResult;
    }

    public static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute == null ? value.ToString() : attribute.Description;
    }

    private static bool ContainsSeparator(string position)
    {
        if (position.Contains(" OF ") || position.Contains(" FOR ") || position.Contains(" TO ") || position.Contains(" AND ") || position.Contains(",")) return true;
        return false;
    }
}
