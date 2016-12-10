using System;
using JetBrains.Annotations;

namespace PachowStudios.Framework.Animation
{
  public class AnimationParameter<T> : IAnimationParameter
  {
    private readonly Func<T> getter;

    public string Name { get; }

    public T Value => this.getter();

    public AnimationParameter([NotNull] string name, [NotNull] Func<T> getter)
    {
      Name = name;
      this.getter = getter;
    }
  }
}
