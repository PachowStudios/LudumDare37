using System;
using PachowStudios.Framework;

namespace PachowStudios.LudumDare37.Guns
{
  public partial class GunInstaller
  {
    [Serializable, InstallerSettings]
    public class Settings
    {
      public GunComponents Components;
    }
  }
}