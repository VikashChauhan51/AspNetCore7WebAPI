namespace CourseLibrary.API.Helpers;

public static class AgeHelper
{

    public static bool BeValidAge(DateTime date)
    {
        int currentYear = DateTimeOffset.Now.Year;
        int dobYear = date.Year;
        int diff = currentYear - dobYear;
        if (diff > 18 && diff < 80)
        {
            return true;
        }

        return false;
    }
}
