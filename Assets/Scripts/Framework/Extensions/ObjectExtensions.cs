using System.Collections.Generic;
using JetBrains.Annotations;

namespace System
{
  public static class ObjectExtensions
  {
    [Pure]
    public static bool IsDefault<T>([CanBeNull] this T @object)
      => EqualityComparer<T>.Default.Equals(@object, default(T));

    [Pure]
    public static bool RefersTo<T1, T2>([CanBeNull] this T1 objectA, [CanBeNull] T2 objectB)
      where T1 : class
      where T2 : class
      => ReferenceEquals(objectA, objectB);

    [Pure]
    public static int ToInt(this bool value)
      => value ? 1 : 0;
  }
}