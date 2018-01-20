using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string ToVariableName(this string target)
    {
        var newTarget = target.ToLower();
        newTarget = Regex.Replace(newTarget, @"\s+", "");
        return newTarget;
    }
}