using System;
using PachowStudios.Framework;
using PachowStudios.LudumDare37.Guns;

namespace PachowStudios.LudumDare37.Player
{
  public partial class PlayerInstaller
  {
    [Serializable, InstallerSettings]
    public class Settings
    {
      public PlayerComponents Components;
      public PlayerMoveHandler.Settings MoveHandler;
      public PlayerGunSelector.Settings GunSelector;
    }
  }
}