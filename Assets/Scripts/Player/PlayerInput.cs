using InControl;
using UnityEngine;

namespace PachowStudios.LudumDare37.Player
{
  public class PlayerInput : PlayerActionSet
  {
    private static KeyBindingSource CancelSetBindingKey { get; } = new KeyBindingSource(Key.Escape);

    public Vector2 Movement => Move.Value;
    public bool IsWalking => Move.IsPressed;
    public Vector2 AimTarget => Input.mousePosition;

    private PlayerTwoAxisAction Move { get; }
    private PlayerAction Shoot { get; }

    public PlayerInput()
    {
      Move = CreateTwoAxisPlayerAction(
        CreatePlayerAction($"{nameof(Move)}Left").WithDefault(Key.A),
        CreatePlayerAction($"{nameof(Move)}Right").WithDefault(Key.D),
        CreatePlayerAction($"{nameof(Move)}Down").WithDefault(Key.S),
        CreatePlayerAction($"{nameof(Move)}Up").WithDefault(Key.W));
      Shoot = CreatePlayerAction(nameof(Shoot)).WithDefault(Mouse.LeftButton);

      ListenOptions = new BindingListenOptions
      {
        IncludeUnknownControllers = false,
        MaxAllowedBindings = 3,
        OnBindingFound = OnBindingFound
      };
    }

    private static bool OnBindingFound(PlayerAction action, BindingSource binding)
    {
      if (binding != CancelSetBindingKey)
        return true;

      action.StopListeningForBinding();

      return false;
    }
  }
}
