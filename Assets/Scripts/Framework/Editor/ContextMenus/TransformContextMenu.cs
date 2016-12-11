using UnityEditor;
using UnityEngine;

namespace PachowStudios.Framework.Editor.ContextMenus
{
  public static class TransformContextMenu
  {
    private const string SnapToPixelGridContextMenu = "CONTEXT/Transform/Snap to pixel grid";

    private static GameObject Target => Selection.activeGameObject;
    private static Transform Transform => Target.GetComponent<Transform>().NullToRealNull();
    private static Sprite Sprite => Target.GetComponent<SpriteRenderer>().NullToRealNull()?.sprite;

    [MenuItem(SnapToPixelGridContextMenu)]
    public static void SnapToPixelGrid()
      => Transform.position = Transform.position.TransformAll(v => v.RoundToFraction((int)Sprite.pixelsPerUnit));

    [MenuItem(SnapToPixelGridContextMenu, true)]
    public static bool ValidateSnapToPixelGrid()
      => Transform != null && Sprite != null;
  }
}
