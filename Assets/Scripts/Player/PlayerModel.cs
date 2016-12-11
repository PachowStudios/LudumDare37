using PachowStudios.Framework.Messaging;
using PachowStudios.Framework.Movement;
using PachowStudios.LudumDare37.Guns;
using UnityEngine;

namespace PachowStudios.LudumDare37.Player
{
  public class PlayerModel
  {
    public Vector2 Velocity { get; set; }
    public GunFacade CurrentGun { get; set; }

    public Transform GunPoint => Components.GunPoint;
    public Vector2 Position => Transform.position;

    public Quaternion Rotation
    {
      get { return Transform.localRotation; }
      set { Transform.localRotation = value; }
    }

    private PlayerComponents Components { get; }
    private IEventAggregator EventAggregator { get; }

    private Transform Transform => Components.Body;
    private IMovementController2D MovementController => Components.MovementController;

    public PlayerModel(PlayerComponents components, IEventAggregator eventAggregator)
    {
      Components = components;
      EventAggregator = eventAggregator;
    }

    public void Move(Vector2 velocity) => Velocity = MovementController.Move(velocity);
  }
}
