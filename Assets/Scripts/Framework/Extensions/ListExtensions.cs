using System.Linq;
using System.Linq.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace System.Collections.Generic
{
  public static class ListExtensions
  {
    [Pure]
    public static int IndexOfFirst<T>([NotNull] this IList<T> list, [NotNull, InstantHandle] Func<T, bool> selector)
    {
      var item = list.FirstOrDefault(selector);

      return item.IsDefault() ? -1 : list.IndexOf(item);
    }

    [CanBeNull, Pure]
    public static T ElementAtWrap<T>([NotNull] this IList<T> list, int index)
      => list[index.Wrap(0, list.Count - 1)];

    [CanBeNull, Pure]
    public static T GetRandom<T>([NotNull] this IList<T> source)
      => source[UnityRandom.Range(0, source.Count)];

    public static void AddRange<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] IEnumerable<T> items)
      => items.ForEach(source.Add);

    [NotNull]
    public static T SingleOrAdd<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] Func<T, bool> predicate)
      where T : class, new()
      => source.SingleOrAdd(predicate, () => new T());

    [NotNull]
    public static T SingleOrAdd<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] Func<T, bool> predicate,
      [NotNull, InstantHandle] Func<T> factory)
      where T : class
    {
      var result = source.SingleOrDefault(predicate);

      if (result == null)
        source.Add(result = factory());

      return result;
    }

    [CanBeNull]
    public static T RemoveSingle<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] Func<T, bool> predicate)
    {
      var item = source.SingleOrDefault(predicate);

      if (Equals(item, default(T)))
        source.Remove(item);

      return item;
    }

    public static void ReplaceAll<T>(
      [NotNull] this IList<T> source,
      [NotNull, InstantHandle] IEnumerable<T> items)
    {
      source.Clear();
      source.AddRange(items);
    }

    [CanBeNull]
    public static T Pop<T>([NotNull] this IList<T> source)
    {
      var lastIndex = source.Count - 1;
      var lastItem = source[lastIndex];

      source.RemoveAt(lastIndex);

      return lastItem;
    }

    public static void SortBy<T, TKey>(
      [NotNull] this List<T> list,
      [NotNull, InstantHandle] Func<T, TKey> keySelector)
      where TKey : IComparable<TKey>
      => list.Sort((i1, i2) => keySelector(i1).CompareTo(keySelector(i2)));
  }
}
