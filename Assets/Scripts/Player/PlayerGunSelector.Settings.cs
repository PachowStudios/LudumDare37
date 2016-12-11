using System;
using PachowStudios.Framework;

namespace PachowStudios.LudumDare37.Guns
{
  public partial class PlayerGunSelector
  {
    [Serializable, InstallerSettings]
    public class Settings
    {
      public GunType StartingGun;
    }
  }
}