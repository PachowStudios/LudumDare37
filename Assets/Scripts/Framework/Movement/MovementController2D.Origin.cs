using UnityEngine;

namespace PachowStudios.Framework.Movement
{
  public abstract partial class MovementController2D : MonoBehaviour, IRaycastSource
  {
    protected class Origin
    {
      public Vector2 TopLeft { get; }
      public Vector2 BottomRight { get; }
      public Vector2 BottomLeft { get; }

      public Origin(Bounds bounds)
      {
        TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        BottomLeft = bounds.min;
      }
    }
  }
}