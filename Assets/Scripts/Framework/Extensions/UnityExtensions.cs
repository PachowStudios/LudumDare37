using System;
using JetBrains.Annotations;
using PachowStudios.Framework.Primitives;
using UnityObject = UnityEngine.Object;

namespace UnityEngine
{
  public static class UnityExtensions
  {
    /// <summary>
    /// Checks if an object is null using Unity's custom null check.
    /// The object must be cast to a Unity Object as a hint to the compiler.
    /// </summary>
    [Pure, ContractAnnotation("null => true")]
    public static bool IsNull([CanBeNull] this object @object)
      // ReSharper disable once MergeSequentialChecks
      // ReSharper disable once TryCastAndCheckForNull.0
      => @object == null || @object as UnityObject == null;

    /// <summary>
    /// Converts Unity's fake null to a real null.
    /// </summary>
    [Pure, ContractAnnotation("null => null")]
    public static T NullToRealNull<T>([CanBeNull] this T unityObject)
      where T : class
      => unityObject.IsNull() ? null : unityObject;

    [CanBeNull]
    public static T GetComponentIfNull<T>([CanBeNull] this Component component, [CanBeNull] ref T target)
      where T : class
      => component.NullToRealNull()?.gameObject.GetComponentIfNull(ref target);

    [CanBeNull]
    public static T GetComponentIfNull<T>([CanBeNull] this GameObject gameObject, [CanBeNull] ref T target)
      where T : class
      => target.NullToRealNull()
         ?? (target = gameObject.NullToRealNull()?.GetComponent<T>().NullToRealNull());

    [CanBeNull]
    public static T GetComponentInParentIfNull<T>([CanBeNull] this Component component, [CanBeNull] ref T target)
      where T : class
      => component.NullToRealNull()?.gameObject.GetComponentInParentIfNull(ref target);

    [CanBeNull]
    public static T GetComponentInParentIfNull<T>([CanBeNull] this GameObject gameObject, [CanBeNull] ref T target)
      where T : class
      => target.NullToRealNull()
         ?? (target = gameObject.NullToRealNull()?.GetComponentInParent<T>().NullToRealNull());

    [CanBeNull]
    public static T GetComponentInChildrenIfNull<T>([CanBeNull] this Component component, [CanBeNull] ref T target)
      where T : class
      => component.NullToRealNull()?.gameObject.GetComponentInChildrenIfNull(ref target);

    [CanBeNull]
    public static T GetComponentInChildrenIfNull<T>([CanBeNull] this GameObject gameObject, [CanBeNull] ref T target)
      where T : class
      => target.NullToRealNull()
         ?? (target = gameObject.NullToRealNull()?.GetComponentInChildren<T>().NullToRealNull());

    [CanBeNull]
    public static T[] GetComponentsInChildrenIfNull<T>([CanBeNull] this Component component, [CanBeNull] ref T[] target)
      where T : class
      => component.NullToRealNull()?.gameObject.GetComponentsInChildrenIfNull(ref target);

    [CanBeNull]
    public static T[] GetComponentsInChildrenIfNull<T>([CanBeNull] this GameObject gameObject, [CanBeNull] ref T[] target)
      where T : class
      => target ?? (target = gameObject.NullToRealNull()?.GetComponentsInChildren<T>());

    public static void ParentTo([NotNull] this Component component, [NotNull] Transform transform)
      => component.transform.parent = transform;

    public static void Unparent([NotNull] this Component component)
      => component.transform.parent = null;

    public static void Destroy([NotNull] this MonoBehaviour monoBehaviour)
      => monoBehaviour.gameObject.Destroy();

    public static void Destroy([NotNull] this GameObject gameObject)
      => UnityObject.Destroy(gameObject);

    public static void DestroyAfter([NotNull] this MonoBehaviour monoBehaviour, float delay)
      => monoBehaviour.gameObject.DestroyAfter(delay);

    public static void DestroyAfter([NotNull] this GameObject gameObject, float delay)
      => UnityObject.Destroy(gameObject, delay);

    [NotNull]
    public static GameObject HideInHierarchy([NotNull] this GameObject gameObject)
    {
      gameObject.hideFlags |= HideFlags.HideInHierarchy;

      gameObject.SetActive(false);
      gameObject.SetActive(true);

      return gameObject;
    }

    [NotNull]
    public static GameObject UnhideInHierarchy([NotNull] this GameObject gameObject)
    {
      gameObject.hideFlags &= ~HideFlags.HideInHierarchy;

      gameObject.SetActive(false);
      gameObject.SetActive(true);

      return gameObject;
    }

    public static void Flash([NotNull] this SpriteRenderer spriteRenderer, Color color, float time)
    {
      spriteRenderer.color = color;

      // The spriteRenderer could be null by the time this executes
      Wait.ForSeconds(time, () => spriteRenderer.NullToRealNull()?.ResetColor());
    }

    public static void ResetColor([NotNull] this SpriteRenderer spriteRenderer)
      => spriteRenderer.color = Color.white;

    public static void DetachAndDestroy([NotNull] this ParticleSystem particleSystem)
    {
      particleSystem.transform.parent = null;
      particleSystem.SetEmissionEnabled(false);
      particleSystem.gameObject.DestroyAfter(particleSystem.startLifetime);
    }

    public static void SetEmissionEnabled([NotNull] this ParticleSystem particleSystem, bool enabled)
    {
      var emission = particleSystem.emission;
      emission.enabled = enabled;
    }

    [Pure]
    public static bool IsVisible([NotNull] this CanvasGroup canvasGroup)
      => !canvasGroup.alpha.IsZero();

    public static void SetVisibility([NotNull] this CanvasGroup canvasGroup, bool visible)
    {
      canvasGroup.alpha = visible.ToInt();
      canvasGroup.interactable = visible;
      canvasGroup.blocksRaycasts = visible;
    }

    [Pure]
    public static bool HasLayer(this LayerMask mask, int layer)
      => (mask.value & (1 << layer)) > 0;

    [Pure]
    public static bool HasLayer(this LayerMask mask, [NotNull] GameObject gameObject)
      => mask.HasLayer(gameObject.layer);

    [Pure]
    public static bool HasLayer(this LayerMask mask, [NotNull] Collider2D collider)
      => mask.HasLayer(collider.gameObject.layer);

    [Pure]
    public static float UnitsToPixels([NotNull] this Camera camera, float units)
      => camera.WorldToScreenPoint(camera.ViewportToWorldPoint(Vector3.zero).Add(x: units)).x;
  }
}