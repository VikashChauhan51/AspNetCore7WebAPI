namespace CourseLibrary.API.Extensions;

public static class DateTimeOffsetExtensions
{
    public static int GetCurrentAge(this DateTimeOffset dateTimeOffset)
    {
        var currentDate = DateTime.UtcNow;
        int age = currentDate.Year - dateTimeOffset.Year;

        if (currentDate < dateTimeOffset.AddYears(age))
        {
            age--;
        }

        return age;
    }
    public static int GetCurrentAge(this DateTime dateTime)
    {
        var currentDate = DateTime.UtcNow;
        int age = currentDate.Year - dateTime.Year;

        if (currentDate < dateTime.AddYears(age))
        {
            age--;
        }

        return age;
    }
    public static int GetCurrentAge(this DateOnly date)
    {
        var currentUtcDate = DateTime.UtcNow;
        var currentDate = new DateOnly(currentUtcDate.Year, currentUtcDate.Month, currentUtcDate.Day);
        int age = currentDate.Year - date.Year;
        if (currentDate < date.AddYears(age))
        {
            age--;
        }

        return age;
    }
}
