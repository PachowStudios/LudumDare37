namespace PachowStudios.Framework.Movement
{
  public partial class MovementController2D
  {
    protected class CollisionState
    {
      public bool Right { get; set; }
      public bool Left { get; set; }
      public bool Up { get; set; }
      public bool Down { get; set; }

      public bool Any => Right || Left || Up || Down;
    }
  }
}