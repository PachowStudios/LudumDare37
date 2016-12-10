using UnityEngine;

namespace PachowStudios.Framework.Movement
{
  public partial class MovementController2D
  {
    private class Origin
    {
      public Vector2 TopLeft { get; }
      public Vector2 BottomRight { get; }
      public Vector2 BottomLeft { get; }

      public Origin(Vector2 topLeft, Vector2 bottomRight, Vector2 bottomLeft)
      {
        TopLeft = topLeft;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
      }
    }
  }
}
