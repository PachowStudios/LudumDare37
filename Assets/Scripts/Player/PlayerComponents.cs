using System;
using PachowStudios.Framework;
using PachowStudios.Framework.Movement;
using UnityEngine;

namespace PachowStudios.LudumDare37.Player
{
  [Serializable, InstallerSettings]
  public class PlayerComponents
  {
    public Transform Body;
    public TopDownMovementController2D MovementController;
    public Animator Animator;
  }
}
