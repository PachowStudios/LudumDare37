using PachowStudios.LudumDare37.Guns;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Player
{
  public class PlayerRotationHandler : ILateTickable
  {
    private PlayerModel Model { get; }

    private GunFacade Gun => Model.CurrentGun;

    public PlayerRotationHandler(PlayerModel model)
    {
      Model = model;
    }

    public void LateTick() => Model.Rotation = Gun.AimDirection.DirectionToRotation2D();
  }
}
