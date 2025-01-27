﻿namespace Common.String;

public static class Extensions
{
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };

    public static string CommaSeparate(this IEnumerable<string> strings)
    {
        return string.Join(',',strings);
    }
    
    public static string CharSeparate(this IEnumerable<string> strings, char c)
    {
        return string.Join(c,strings);
    }
}