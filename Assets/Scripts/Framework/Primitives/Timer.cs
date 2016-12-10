using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace PachowStudios.Framework.Primitives
{
  public class Timer
  {
    private float remainingTime;

    public float RemainingTime
    {
      get { return this.remainingTime; }
      private set { this.remainingTime = Mathf.Max(0f, value); }
    }

    public float Interval { get; private set; }

    private float MinInterval { get; }
    private float MaxInterval { get; }
    private Action OnCompleted { get; }

    public Timer(float interval, [NotNull] Action onCompleted)
      : this(interval, interval, onCompleted) { }

    public Timer(float minInterval, float maxInterval, [NotNull] Action onCompleted)
    {
      MinInterval = minInterval;
      MaxInterval = maxInterval;
      OnCompleted = onCompleted;

      Restart();
    }

    public void Tick(float deltaTime)
    {
      RemainingTime -= deltaTime;

      if (RemainingTime <= 0f)
      {
        OnCompleted();
        Restart();
      }
    }

    public void Restart()
      => RemainingTime = Interval = UnityRandom.Range(MinInterval, MaxInterval);
  }
}
