using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PachowStudios.Framework.Assertions;
using PachowStudios.Framework.Primitives;
using UnityEngine;
using Zenject;

namespace PachowStudios.Framework.Animation
{
  public class AnimationController : IAnimationController, ITickable
  {
    private readonly List<IAnimationParameter> parameters = new List<IAnimationParameter>();
    private readonly Animator animator;
    private readonly TypeSwitch parameterSwitch;

    public AnimationController([NotNull] Animator animator)
    {
      this.animator = animator;
      this.parameterSwitch = new TypeSwitch()
        .Case<AnimationParameter<bool>>(p => this.animator.SetBool(p.Name, p.Value))
        .Case<AnimationParameter<int>>(p => this.animator.SetInteger(p.Name, p.Value))
        .Case<AnimationParameter<float>>(p => this.animator.SetFloat(p.Name, p.Value));
    }

    public AnimationController Add(string name, Func<bool> getter)
      => Add<bool>(name, getter);

    public AnimationController Add(string name, Func<int> getter)
      => Add<int>(name, getter);

    public AnimationController Add(string name, Func<float> getter)
      => Add<float>(name, getter);

    [NotNull]
    private AnimationController Add<T>([NotNull] string name, [NotNull] Func<T> getter)
    {
      this.parameters.Should().HaveNoneWhere(p => p.Name.EqualsIgnoreCase(name), "because parameter names must be unique.");
      this.parameters.Add(new AnimationParameter<T>(name, getter));

      return this;
    }

    public void Trigger(string name)
      => this.animator.SetTrigger(name);

    public void Tick()
      => this.parameters.ForEach(this.parameterSwitch.Switch);
  }
}
