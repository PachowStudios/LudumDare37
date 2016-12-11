using PachowStudios.Framework.Messaging;
using PachowStudios.Framework.Movement;
using UnityEngine;

namespace PachowStudios.LudumDare37.Player
{
  public class PlayerModel
  {
    public Vector2 Velocity { get; set; }

    private PlayerComponents Components { get; }
    private IEventAggregator EventAggregator { get; }

    private Transform Transform => Components.Body;
    private TopDownMovementController2D MovementController => Components.MovementController;

    public PlayerModel(PlayerComponents components, IEventAggregator eventAggregator)
    {
      Components = components;
      EventAggregator = eventAggregator;
    }

    public void Move(Vector2 velocity)
      => Velocity = MovementController.Move(velocity);
  }
}
