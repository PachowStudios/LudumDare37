using System.Text;
using JetBrains.Annotations;

namespace System
{
  public static class StringExtensions
  {
    [Pure, ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty([CanBeNull] this string @string)
      => string.IsNullOrEmpty(@string);

    [Pure]
    public static bool EqualsIgnoreCase([NotNull] this string @string, string other)
      => @string.Equals(other, StringComparison.InvariantCultureIgnoreCase);

    [NotNull, Pure]
    public static string Head([NotNull] this string @string, int count)
      => @string.Substring(0, count);

    [NotNull, Pure]
    public static string Tail([NotNull] this string @string, int count)
      => @string.Substring(@string.Length - count);

    [NotNull, Pure]
    public static string Repeat([NotNull] this string @string, int count)
      => new StringBuilder(@string.Length * count).Insert(0, @string, count).ToString();

    [NotNull, Pure]
    public static string StartWith(
        [NotNull] this string @string,
        [NotNull] string startingString,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => @string.StartsWith(startingString, comparison) ? @string : startingString + @string;

    [NotNull, Pure]
    public static string EndWith(
        [NotNull] this string @string,
        [NotNull] string endingString,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
      => @string.EndsWith(endingString, comparison) ? @string : @string + endingString;

    [Pure]
    public static T ToEnumOrDefault<T>([NotNull] this string @string, bool ignoreCase = true)
    {
      try
      {
        return (T)Enum.Parse(typeof(T), @string, ignoreCase);
      }
      catch
      {
        return default(T);
      }
    }
  }
}