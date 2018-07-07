using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class ExtensionMethods
{
    public static string ToDelimitedString<T>(this IEnumerable<T> source)
    {
        return source.ToDelimitedString(x => x.ToString(),
            CultureInfo.CurrentCulture.TextInfo.ListSeparator);
    }

    public static string ToDelimitedString<T>(
        this IEnumerable<T> source, Func<T, string> converter)
    {
        return source.ToDelimitedString(converter,
            CultureInfo.CurrentCulture.TextInfo.ListSeparator);
    }

    public static string ToDelimitedString<T>(
        this IEnumerable<T> source, string separator)
    {
        return source.ToDelimitedString(x => x.ToString(), separator);
    }

    public static string ToDelimitedString<T>(this IEnumerable<T> source,
        Func<T, string> converter, string separator)
    {
        return string.Join(separator, source.Select(converter).ToArray());
    }

}
