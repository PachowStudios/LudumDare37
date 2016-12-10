using System;

namespace PachowStudios.Framework
{
  public static class EnumHelper
  {
    public static T[] GetValues<T>()
      => (T[])Enum.GetValues(typeof(T));
  }
}
