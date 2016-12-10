using System.Collections.Generic;
using JetBrains.Annotations;
using PachowStudios.Framework.Primitives;

// Needs to be in a sub-namespace so we don't conflict with plugins that have the same extensions
namespace System.Linq.Extensions
{
  public static class LinqExtensions
  {
    [Pure]
    public static bool IsEmpty<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => !source.Any();

    [Pure, ContractAnnotation("null => true")]
    public static bool IsNullOrEmpty<T>([CanBeNull, InstantHandle] this IEnumerable<T> source)
      => source == null || source.IsEmpty();

    [Pure]
    public static bool None<T>(
      [NotNull, InstantHandle] this IEnumerable<T> source,
      [NotNull, InstantHandle] Func<T, bool> condition)
      => !source.Any(condition);

    [Pure]
    public static bool HasAtLeast<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.Skip(amount).Any();

    [Pure]
    public static bool HasMoreThan<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.HasAtLeast(amount + 1);

    [Pure]
    public static bool HasAtMost<T>([NotNull, InstantHandle] this IEnumerable<T> souce, int amount)
      => !souce.Skip(amount).Any();

    [Pure]
    public static bool HasLessThan<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.HasAtMost(amount - 1);

    [Pure]
    public static bool HasExactly<T>([NotNull, InstantHandle] this IEnumerable<T> source, int amount)
      => source.Take(amount + 1).Count() == amount;

    [Pure]
    public static bool HasSingle<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => source.HasExactly(1);

    [Pure]
    public static bool HasMultiple<T>([NotNull, InstantHandle] this IEnumerable<T> source)
      => source.HasAtLeast(2);

    public static void ForEach<T>(
      [NotNull, InstantHandle] this IEnumerable<T> source,
      [CanBeNull, InstantHandle] Action<T> action)
    {
      foreach (var item in source)
        action?.Invoke(item);
    }

    [NotNull, LinqTunnel]
    public static IEnumerable<T> Do<T>([NotNull] this IEnumerable<T> source, [NotNull] Action<T> action)
    {
      foreach (var item in source)
      {
        action(item);
        yield return item;
      }
    }

    [NotNull, LinqTunnel]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
      [NotNull] this IEnumerable<TSource> source,
      [NotNull] Func<TSource, TKey> selector)
      => new HashSet<TSource>(source, LambdaEqualityComparer<TSource>.Create(selector));

    [CanBeNull, Pure]
    public static TSource Lowest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector)
      => source.Lowest(selector, Comparer<TKey>.Default);

    [CanBeNull, Pure]
    public static TSource Lowest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector,
      [NotNull, InstantHandle] IComparer<TKey> comparer)
      => source.Aggregate(
        (low, c) => comparer.Compare(selector(low), selector(c)) < 0 ? low : c);

    [CanBeNull, Pure]
    public static TSource Highest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector)
      => source.Highest(selector, Comparer<TKey>.Default);

    [CanBeNull, Pure]
    public static TSource Highest<TSource, TKey>(
      [NotNull, InstantHandle] this IEnumerable<TSource> source,
      [NotNull, InstantHandle] Func<TSource, TKey> selector,
      [NotNull, InstantHandle] IComparer<TKey> comparer)
      => source.Aggregate(
        (high, c) => comparer.Compare(selector(high), selector(c)) > 0 ? high : c);

    [NotNull, Pure, LinqTunnel]
    public static IEnumerable<T> Shuffle<T>([NotNull] this IEnumerable<T> source)
      => source.OrderBy(x => Guid.NewGuid());

    [NotNull, Pure]
    public static string ToValuesString<T>([NotNull] this IEnumerable<T> source)
      => string.Join(", ", source.Select(x => x.ToString()).ToArray());
  }
}