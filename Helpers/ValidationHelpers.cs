using System.ComponentModel;
using System.Text.RegularExpressions;
using WhiteHouseETL.Models;

namespace WhiteHouseETL.Helpers;

public static class ValidationHelpers
{
    public static bool EmployeeTableValidation(string name)
    {

        bool isMatch = false;

        if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\.,[A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\. ")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+ [A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.+ [A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.+ [A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+ [A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.,[A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+,[A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]+ [A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+ [A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+\.,[A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]+,[A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]+ [A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+ [A-Za-z]+,[A-Za-z]")) isMatch = true;
        else if (Regex.IsMatch(name, @"^[A-Za-z\-']+,[A-Za-z]")) isMatch = true;
        
       

        return isMatch;
    }

    public static bool PositionTableValidation(string position, string payBasis, string status)
    {
        bool positionValid = false;
        bool payBasisValid = false;
        bool statusValid = false;

        int invalidCharacters = 0;
        int ofIndex = position.IndexOf(" OF ");
        int forIndex = position.IndexOf(" FOR ");
        int toIndex = position.IndexOf(" TO ");
        int andIndex = position.IndexOf(" AND ");

        foreach (char ch in position)
        {
            if (!(Char.IsLetter(ch) || ch == ' ' || ch == ',' || ch == '\''))
            {
                invalidCharacters++;
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

        if (positionValid == true && payBasisValid == true && statusValid == true && invalidCharacters == 0 
            && !ValidationHelpers.ContainsSeparator(position) ) return true;
        else return false;
    }

    public static (List<WhiteHouseStaff>, List<WhiteHouseStaff>) WhiteHouseStaffFileValidation(List<WhiteHouseStaff> records) 
    {
        List<WhiteHouseStaff> validRecords = new List<WhiteHouseStaff>();
        List<WhiteHouseStaff> invalidRecords = new List<WhiteHouseStaff>();

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
                invalidRecords.Add(new WhiteHouseStaff()
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
        }

        return (validRecords, invalidRecords);
    }

    public static bool SalaryTableValidation(int salary, int year)
    {
        if (salary > 0 && year > 1900) return true;
        else return false;
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
