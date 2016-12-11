using System;
using PachowStudios.Framework;

namespace PachowStudios.LudumDare37.Player
{
  public partial class PlayerMoveHandler
  {
    [Serializable, InstallerSettings]
    public class Settings
    {
      public float WalkSpeed = 10f;
      public float Acceleration = 15f;
    }
  }
}
