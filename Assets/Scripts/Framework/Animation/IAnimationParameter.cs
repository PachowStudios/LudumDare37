using JetBrains.Annotations;

namespace PachowStudios.Framework.Animation
{
  public interface IAnimationParameter
  {
    [NotNull] string Name { get; }
  }
}