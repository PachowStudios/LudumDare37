using UnityEngine;

namespace PachowStudios.Framework.Camera.Effectors
{
  [AddComponentMenu("Pachow Studios/Camera/Effectors/Cue Focus Polygon")]
  [RequireComponent(typeof(PolygonCollider2D))]
  public class CueFocusPolygon : CueFocusBase
  {
    protected new PolygonCollider2D EffectorTrigger => (PolygonCollider2D)base.EffectorTrigger;

    public override float GetEffectorWeight() => EffectorWeight;
  }
}
