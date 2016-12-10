using System;
using JetBrains.Annotations;
using UnityEngine;

namespace PachowStudios.Framework.Primitives
{
  public class Lazy<T>
    where T : class
  {
    private readonly Func<T> valueFactory;

    private T value;

    [NotNull] public T Value => HasValue ? this.value : (this.value = CreateValue());

    // This null check is done with our IsNull extension
    // because MonoBehavior's custom null check doesn't work
    // with unconstrained generics...
    private bool HasValue => !this.value.IsNull();

    public Lazy(Func<T> valueFactory = null)
    {
      this.valueFactory = valueFactory;
    }

    private T CreateValue()
      => this.valueFactory?.Invoke()
         ?? Activator.CreateInstance<T>();

    public static implicit operator T([NotNull] Lazy<T> @this)
      => @this.Value;
  }

  public static class Lazy
  {
    [NotNull, Pure]
    public static Lazy<T> From<T>([NotNull] Func<T> func)
      where T : class
      => new Lazy<T>(func);
  }
}