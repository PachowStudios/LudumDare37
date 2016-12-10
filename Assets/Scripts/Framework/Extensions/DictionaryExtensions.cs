using JetBrains.Annotations;

namespace System.Collections.Generic
{
  public static class DictionaryExtensions
  {
    [NotNull]
    public static TValue GetOrAdd<TKey, TValue>(
      [NotNull] this IDictionary<TKey, TValue> source,
      [NotNull] TKey key,
      [NotNull, InstantHandle] Func<TValue> factory)
      => source.GetOrAdd(key, k => factory());

    [NotNull]
    public static TValue GetOrAdd<TKey, TValue>(
      [NotNull] this IDictionary<TKey, TValue> source,
      [NotNull] TKey key,
      [NotNull, InstantHandle] Func<TKey, TValue> factory)
    {
      TValue result;

      if (!source.TryGetValue(key, out result))
        source[key] = factory(key);

      return source[key];
    }
  }
}
