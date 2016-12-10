namespace PachowStudios.Framework.Movement
{
  public partial class MovementController2D
  {
    private class CollisionState
    {
      public bool Right { get; set; }
      public bool Left { get; set; }
      public bool Above { get; set; }
      public bool Below { get; set; }
      public bool BecameGroundedThisFrame { get; set; }
      public bool WasGroundedLastFrame { get; set; }
      public bool IsMovingDownSlope { get; set; }
      public float SlopeAngle { get; set; }

      public bool Any => Right || Left || Above || Below;

      public void Reset()
      {
        Right = Left = Above = Below = BecameGroundedThisFrame = IsMovingDownSlope = false;
        SlopeAngle = 0f;
      }
    }
  }
}
