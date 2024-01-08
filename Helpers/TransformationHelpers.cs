using System.Text.RegularExpressions;

namespace WhiteHouseETL.Helpers;

public static class TransformationHelpers
{
    public static string ExtractNameWithDeLimiters(string text, string type = "LEFT", string delimiterLeft = "", string delimiterRight = "")
    {
        string extractedText = "";
        int delimterLeftIdx;
        int delimterRightIdx;
        int length;
        int commas = text.Count(c => c == ',');
        int spaces = text.Count(c => c == ' ');

        //if (commas == 2 && spaces == 2) Console.WriteLine(text);

        text = text.Trim();

        try
        {
            switch (type)
            {
                case "RIGHT":
                    delimterRightIdx = text.IndexOf(delimiterRight);

                    length = text.Length - (delimterRightIdx + 1);

                    if (length > 0) 
                    {
                        if (commas == 1 && spaces == 0) extractedText = "";
                        else if (commas == 1 && spaces == 2)
                        {
                            int comma = text.IndexOf(',');
                            int space = text.IndexOf(' ');
                            int secondSpace = text.IndexOf(' ', text.IndexOf(' ') + 1);

                            if (comma < space && comma < secondSpace) text.Substring(delimterRightIdx + 1, length);
                            else if (comma > space && comma < secondSpace)
                            {
                                delimterRightIdx = secondSpace;
                                length = text.Length - (delimterRightIdx + 1);
                                extractedText = text.Substring(delimterRightIdx + 1, length).Substring(0,1);
                            }
                        }
                        else if (commas == 2 && spaces == 0) extractedText = "";
                        else if (commas == 2 && spaces == 2) 
                        {
                            delimterRightIdx = text.IndexOf(' ', text.IndexOf(' ') + 1);
                            length = text.Length - (delimterRightIdx + 1);
                            extractedText = text.Substring(delimterRightIdx + 1, length).Substring(0,1);
                        } 
                        else extractedText = text.Substring(delimterRightIdx + 1, length).Substring(0,1);
                    }
                    break;
                case "BETWEEN":
                    delimterLeftIdx = text.IndexOf(delimiterLeft);

                    if (commas == 1 && spaces == 0) delimterRightIdx = text.Length - 1;
                    else if (commas == 1 && spaces == 1)
                    {
                        int comma = text.IndexOf(',');
                        int space = text.IndexOf(' ');

                        if (space < comma)
                        {
                            delimterRightIdx = text.Length - 1;
                        }
                        else delimterRightIdx = text.IndexOf(delimiterRight);

                    }
                    else if (commas == 1 && spaces == 2) delimterRightIdx = text.IndexOf(' ', text.IndexOf(' ') + 1);
                    else if (commas == 1 && spaces == 3) delimterRightIdx = text.IndexOf(' ', text.IndexOf(' ', text.IndexOf(' ') + 1) + 1);
                    else if (commas == 2 && spaces == 0) delimterRightIdx = text.IndexOf(',', text.IndexOf(',') + 1);
                    else if (commas == 2 && spaces == 2) delimterRightIdx = text.IndexOf(' ', text.IndexOf(' ') + 1);
                    else delimterRightIdx = text.IndexOf(delimiterRight);

                    if (delimterRightIdx < delimterLeftIdx)
                    {
                        int transfer = delimterLeftIdx;
                        delimterLeftIdx = delimterRightIdx;
                        delimterRightIdx = transfer;
                    }

                    length = delimterRightIdx - delimterLeftIdx;


                    if (length > 0) 
                    {
                        extractedText = text.Substring(delimterLeftIdx + 1, length);
                    }               
                    break;
                default:
                    delimterLeftIdx = text.IndexOf(delimiterLeft);

                    commas = text.Count(c => c == ',');
                    spaces = text.Count(c => c == ' ');

                    if (commas == 1 && spaces == 0) delimterLeftIdx = text.IndexOf(delimiterLeft);
                    else delimterLeftIdx = text.IndexOf(delimiterLeft);

                    length = delimterLeftIdx;

                    if (length> 0)
                    {
                        extractedText = text.Substring(0, delimterLeftIdx);
                    }    
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.Source);
        }


        return extractedText;
    }

    public static string[] SplitPositionTitle(string positionTitle)
    {
        int ofIndex;
        int forIndex;
        int toIndex;
        int commaIndex;
        int andIndex;
        string splitOn = "";

        positionTitle = positionTitle.Replace("&", "AND");
        positionTitle = positionTitle.Replace(" - ", " AND ");
        positionTitle = positionTitle.Replace("/", " AND ");
        positionTitle = positionTitle.Replace("-", " ");
        positionTitle = positionTitle.Replace(".", "");
        positionTitle = Regex.Replace(positionTitle, @"\([^)]*\)", string.Empty);

        ofIndex = positionTitle.IndexOf(" OF ");
        forIndex = positionTitle.IndexOf(" FOR ");
        toIndex = positionTitle.IndexOf(" TO ");
        commaIndex = positionTitle.IndexOf(", ");
        andIndex = positionTitle.IndexOf(" AND ");


        int selectedIndex =
            Math.Min(
                Math.Min(
                    Math.Min(
                         Math.Min(ofIndex >= 0 ? ofIndex : int.MaxValue, forIndex >= 0 ? forIndex : int.MaxValue), toIndex >= 0 ? toIndex : int.MaxValue),
                    commaIndex >= 0 ? commaIndex : int.MaxValue),
                andIndex >= 0 ? andIndex : int.MaxValue);

        if (selectedIndex == ofIndex) splitOn = " OF ";
        if (selectedIndex == forIndex) splitOn = " FOR ";
        if (selectedIndex == toIndex) splitOn = " TO ";
        if (selectedIndex == commaIndex) splitOn = ", ";
        if (selectedIndex == andIndex && ofIndex == -1 && forIndex == -1 && toIndex == -1 && commaIndex == -1) splitOn = " AND ";

        string[] roles = positionTitle.Split(splitOn);

        //Console.WriteLine(splitOn);
        //Console.WriteLine(selectedIndex + " " + ofIndex + " " + forIndex + " " + toIndex + " " + commaIndex + " " + andIndex);

        if (splitOn == " AND ") return roles;
        else return new string[] { roles[0] };
    }
}
