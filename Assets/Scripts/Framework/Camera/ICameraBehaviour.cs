using UnityEngine;

namespace PachowStudios.Framework.Camera
{
  public interface ICameraBehaviour : ICameraPositionAssertion
  {
    bool IsEnabled { get; }

#if UNITY_EDITOR
    // Useful for while we are in the editor to provide gizmos
    void OnDrawGizmosInternal(Vector3 basePosition);
#endif
  }
}
