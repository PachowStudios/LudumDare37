using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PachowStudios.Framework.Assertions;

namespace PachowStudios.Framework.Primitives
{
  public class TypeSwitch
  {
    private readonly Dictionary<Type, Action<object>> cases = new Dictionary<Type, Action<object>>();

    [NotNull]
    public TypeSwitch Case<T>([NotNull] Action<T> action)
    {
      this.cases.Should().NotContainKey(typeof(T), "because type cases must be unique.");
      this.cases.Add(typeof(T), x => action((T)x));

      return this;
    }

    [NotNull]
    public TypeSwitch Default([NotNull] Action<object> action)
    {
      this.cases.Should().NotContainKey(typeof(DefaultCase), "because the default case can only be set once.");
      this.cases.Add(typeof(DefaultCase), action);

      return this;
    }

    public void Switch([NotNull] object target)
    {
      Action<object> action;

      if (this.cases.TryGetValue(target.GetType(), out action)
          || this.cases.TryGetValue(typeof(DefaultCase), out action))
        action(target);
    }

    private class DefaultCase
    {
      private DefaultCase() { }
    }
  }
}
