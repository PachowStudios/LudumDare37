using UnityEngine;

namespace PachowStudios.Framework.Primitives
{
  public class Cooldown
  {
    private float remainingTime;

    public float RemainingTime
    {
      get { return this.remainingTime; }
      private set { this.remainingTime = Mathf.Max(0f, value); }
    }

    public float Duration { get; }

    public bool IsExpired => RemainingTime <= 0f;

    public Cooldown(float duration)
    {
      Duration = duration;
    }

    public void Tick(float deltaTime)
      => RemainingTime -= deltaTime;

    public void Reset()
      => RemainingTime = Duration;
  }
}
