using JetBrains.Annotations;

namespace InControl
{
  public static class InControlExtensions
  {
    [NotNull]
    public static PlayerAction WithDefault([NotNull] this PlayerAction action, [NotNull] BindingSource binding)
    {
      action.AddDefaultBinding(binding);
      return action;
    }

    [NotNull]
    public static PlayerAction WithDefault([NotNull] this PlayerAction action, InputControlType control)
    {
      action.AddDefaultBinding(control);
      return action;
    }

    [NotNull]
    public static PlayerAction WithDefault([NotNull] this PlayerAction action, params Key[] keys)
    {
      action.AddDefaultBinding(keys);
      return action;
    }

    [NotNull]
    public static PlayerAction WithDefault([NotNull] this PlayerAction action, Mouse control)
    {
      action.AddDefaultBinding(control);
      return action;
    }
  }
}