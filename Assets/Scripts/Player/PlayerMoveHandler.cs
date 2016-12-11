using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Player
{
  public partial class PlayerMoveHandler : ILateTickable
  {
    private Settings Config { get; }
    private PlayerModel Model { get; }
    private PlayerInput Input { get; }

    private float MoveSpeed => Config.WalkSpeed;
    private float Acceleration => Config.Acceleration;
    private Vector2 Velocity => Model.Velocity;

    public PlayerMoveHandler(Settings config, PlayerModel model, PlayerInput input)
    {
      Config = config;
      Model = model;
      Input = input;
    }

    public void LateTick()
      => Model.Move(
        Velocity.LerpTo(Input.Movement * MoveSpeed, Acceleration * Time.deltaTime));
  }
}
