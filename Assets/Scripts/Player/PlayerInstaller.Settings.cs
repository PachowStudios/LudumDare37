using System;
using PachowStudios.Framework;

namespace PachowStudios.LudumDare37.Player
{
  public partial class PlayerInstaller
  {
    [Serializable, InstallerSettings]
    public class Settings
    {
      public PlayerComponents Components;
      public PlayerMoveHandler.Settings MoveHandler;
    }
  }
}