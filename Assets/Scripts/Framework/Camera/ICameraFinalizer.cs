using UnityEngine;

namespace PachowStudios.Framework.Camera
{
  public interface ICameraFinalizer
  {
    bool IsEnabled { get; }
    int GetFinalizerPriority { get; }
    bool ShouldSkipSmoothingThisFrame { get; }

    Vector3 GetFinalCameraPosition(Bounds targetBounds, Vector3 currentCameraPosition, Vector3 desiredCameraPosition);

#if UNITY_EDITOR
    void OnDrawGizmosInternal(Vector3 basePosition);
#endif
  }
}
