using System;
using JetBrains.Annotations;

namespace PachowStudios.Framework
{
  [AttributeUsage(AttributeTargets.Method)]
  [MeansImplicitUse(ImplicitUseKindFlags.Access)]
  public class AnimationEventAttribute : Attribute { }
}