using PachowStudios.LudumDare37.Guns;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Player
{
  public class PlayerShootHandler : ITickable
  {
    private PlayerModel Model { get; }
    private PlayerInput Input { get; }
    private Camera Camera { get; }

    private GunFacade Gun => Model.CurrentGun;
    private Vector2 WorldAimTarget => Camera.ScreenToWorldPoint(Input.AimTarget);

    public PlayerShootHandler(PlayerModel model, PlayerInput input, Camera camera)
    {
      Model = model;
      Input = input;
      Camera = camera;
    }

    public void Tick()
    {
      Gun.IsShooting = Input.IsShooting;
      Gun.AimDirection = WorldAimTarget - Model.Position;
    }
  }
}
