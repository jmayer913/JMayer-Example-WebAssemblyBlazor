namespace JMayer.Example.WebAssemblyBlazor.Client.Extensions;

/// <summary>
/// The static class contains extension methods for the string class.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// The method returns a string where spaces have been added before the capatial letters.
    /// </summary>
    /// <param name="value">The string to space.</param>
    /// <returns>A string.</returns>
    public static string SpaceCapitalLetters(this string value)
    {
        return string.Concat(value.Select(c => char.IsUpper(c) ? $" {c}" : c.ToString())).TrimStart();
    }
}
