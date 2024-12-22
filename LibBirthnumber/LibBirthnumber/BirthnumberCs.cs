using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LibBirthnumber;

public enum BirthNumberGenders
{
    Female,
    Male
}

/// <summary>
/// Czech birth number parser.
/// </summary>
public partial class BirthnumberCs
{
    private string Suffix { get; set; }
    private string YearMonthDay { get; set; }
    private DateTime? date;  
    private BirthNumberGenders? gender;
    private string normalized;

    public BirthNumberGenders Gender
    {
        get
        {
            if (gender.HasValue)
            {
                return gender.Value;
            }

            if (!int.TryParse(YearMonthDay.AsSpan(2, 2), out int monthCode))
            {
                gender = BirthNumberGenders.Male;
            }
            else
            {
                gender = monthCode > 50 ? BirthNumberGenders.Female : BirthNumberGenders.Male;
            }

            return gender.Value;
        }
    }
    
    public DateTime Date
    {
        get
        {
            date ??= ParseDate();
            return date ?? DateTime.MinValue;
        }
    }
    
    private DateTime? ParseDate()
    {
        try
        {
            if (!int.TryParse(YearMonthDay[..2], out int year))
                return null;
            
            year += year < 54 ? 2000 : 1900;
            
            if (!int.TryParse(YearMonthDay.AsSpan(2, 2), out int month))
                return null;
            
            month = month switch
            {
                > 70 => month - 70,
                > 50 => month - 50,
                > 20 => month - 20,
                _ => month
            };

            if (month is < 1 or > 12)
                return null;
            
            if (!int.TryParse(YearMonthDay.AsSpan(4, 2), out int day))
                return null;
            
            if (!IsValidDate(year, month, day))
                return null;

            return new DateTime(year, month, day);
        }
        catch
        {
            return null;
        }
    }
    
    private static bool IsValidDate(int year, int month, int day)
    {
        return day >= 1 && day <= DateTime.DaysInMonth(year, month);
    }
    
    public DateTime GetDate() => Date;
    public int Year => Date.Year;
    public int Month => Date.Month;
    public int Day => Date.Day;
    public BirthNumberGenders GetGender() => Gender;

    public string Text(bool includeDivider = true)
    {
        return includeDivider ? normalized : $"{YearMonthDay}{Suffix}";
    }

    private BirthnumberCs()
    {
        
    }
    
    private BirthnumberCs(string yearMonthDay, string suffix)
    {
        YearMonthDay = yearMonthDay;
        Suffix = suffix;
        normalized = $"{YearMonthDay}/{Suffix}";
    }

    private bool CheckChecksum()
    {
        try
        {
            string cleanYearMonthDay = YearMonthDay.Replace("/", string.Empty);
            
            if (long.TryParse(cleanYearMonthDay + Suffix, out long fullNumber))
            {
                if (fullNumber % 11 is 0)
                {
                    return true;
                }
            }
            
            if (Suffix is not [_, _, _, '0'])
            {
                return false;
            }
        
            string truncatedNumber = string.Concat(cleanYearMonthDay, Suffix.AsSpan(0, 3));
        
            if (long.TryParse(truncatedNumber, out long truncated))
            {
                return truncated % 11 is 10;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private bool CheckDate()
    {
        return Date != DateTime.MinValue && Date != DateTime.MaxValue;
    }

    private static readonly Regex Validate = ValidateRegex();

    /// <summary>
    /// Attempts to parse given string into a birth number.
    /// </summary>
    /// <param name="str">String to parse</param>
    /// <param name="birthNumber">Output</param>
    /// <returns></returns>
    public static bool TryParse(string? str, [NotNullWhen(true)] out BirthnumberCs? birthNumber)
    {
        birthNumber = null;
        
        if (str is null)
        {
            return false;
        }
        
        Match match = Validate.Match(str.Trim());

        if (!match.Success)
        {
            return false;
        }

        BirthnumberCs parsed = new BirthnumberCs(match.Groups[0].Value, match.Groups[4].Value);
        birthNumber = parsed;

        return parsed.CheckChecksum() && parsed.CheckDate();
    }

    /// <summary>
    /// Checks whether given input is a valid birth number.
    /// </summary>
    /// <param name="birthNumber"></param>
    /// <returns></returns>
    public static bool Valid(string birthNumber)
    {
        return TryParse(birthNumber, out BirthnumberCs? _);
    }

    [GeneratedRegex("^([0-9]{6})/?([0-9]{3,4})$", RegexOptions.Compiled)]
    private static partial Regex ValidateRegex();

    public override string ToString()
    {
        return normalized;
    }
}