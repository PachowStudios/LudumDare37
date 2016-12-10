using System;
using JetBrains.Annotations;

namespace PachowStudios.Framework.Animation
{
  public interface IAnimationController
  {
    [NotNull]
    AnimationController Add([NotNull] string name, [NotNull] Func<bool> getter);

    [NotNull]
    AnimationController Add([NotNull] string name, [NotNull] Func<int> getter);

    [NotNull]
    AnimationController Add([NotNull] string name, [NotNull] Func<float> getter);

    void Trigger([NotNull] string name);
  }
}
