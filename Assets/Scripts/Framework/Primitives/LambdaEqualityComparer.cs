using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PachowStudios.Framework.Primitives
{
  public class LambdaEqualityComparer<TSource, TKey> : IEqualityComparer<TSource>
  {
    private IEqualityComparer<TKey> KeyComparer { get; } = EqualityComparer<TKey>.Default;
    private Func<TSource, TKey> Selector { get; }

    public LambdaEqualityComparer(Func<TSource, TKey> selector)
    {
      Selector = selector;
    }

    public bool Equals([CanBeNull] TSource a, [CanBeNull] TSource b)
      => KeyComparer.Equals(Selector(a), Selector(b));

    public int GetHashCode(TSource @object)
      => KeyComparer.GetHashCode(Selector(@object));
  }

  public static class LambdaEqualityComparer<TSource>
  {
    public static LambdaEqualityComparer<TSource, TKey> Create<TKey>(Func<TSource, TKey> selector)
      => new LambdaEqualityComparer<TSource, TKey>(selector);
  }
}
