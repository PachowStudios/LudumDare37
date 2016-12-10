using UnityEngine;

namespace PachowStudios.Framework.Camera
{
  public interface ICameraPositionAssertion
  {
    Vector3 GetDesiredPositionDelta(Bounds targetBounds, Vector3 basePosition, Vector3 targetAverageVelocity);
  }
}
