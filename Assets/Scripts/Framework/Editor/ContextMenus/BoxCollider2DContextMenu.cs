using UnityEditor;
using UnityEngine;

namespace PachowStudios.Framework.Editor.ContextMenus
{
  public static class BoxCollider2DContextMenu
  {
    private const string ResizeToSpriteBorderContextMenu = "CONTEXT/BoxCollider2D/Resize to sprite border";

    private static GameObject Target => Selection.activeGameObject;
    private static BoxCollider2D Collider => Target.GetComponent<BoxCollider2D>().NullToRealNull();
    private static Sprite Sprite => Target.GetComponent<SpriteRenderer>().NullToRealNull()?.sprite;

    [MenuItem(ResizeToSpriteBorderContextMenu)]
    public static void ResizeToSpriteBorder()
    {
      var size = Sprite.bounds.size;
      var extents = Sprite.border / Sprite.pixelsPerUnit;
      var width = size.x - extents.x - extents.z;
      var height = size.y - extents.w - extents.y;
      var anchor = Sprite.pivot.Divide(size) / Sprite.pixelsPerUnit;

      Collider.size = new Vector2(width, height);
      Collider.offset = extents
        .ToVector2()
        .Add(Collider.size / 2f)
        .Subtract(anchor.Multiply(size));
    }

    [MenuItem(ResizeToSpriteBorderContextMenu, true)]
    public static bool ValidateResizeToSpriteBorder()
      => Collider != null && Sprite != null && !Sprite.border.IsZero();
  }
}
