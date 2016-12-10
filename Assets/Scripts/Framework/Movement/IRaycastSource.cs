using System;
using UnityEngine;

namespace PachowStudios.Framework.Movement
{
  public interface IRaycastSource
  {
    event Action<RaycastHit2D> Collided;
    event Action<RaycastHit2D> Triggered;
  }
}
