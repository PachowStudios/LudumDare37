using System;

namespace PachowStudios.Framework.Camera
{
  [Flags]
  public enum CameraAxis
  {
    Horizontal = 1 << 0,
    Vertical   = 1 << 1
  }
}
