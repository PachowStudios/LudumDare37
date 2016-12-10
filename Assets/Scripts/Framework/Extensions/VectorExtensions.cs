using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PachowStudios.Framework;

namespace UnityEngine
{
  public static class VectorExtensions
  {
    [Pure]
    public static Vector3 ToVector3(this Vector2 vector)
      => new Vector3(vector.x, vector.y, 0f);

    [Pure]
    public static Vector2 ToVector2(this Vector4 vector)
      => vector;

    [Pure]
    public static Quaternion ToQuaternion(this Vector3 vector)
      => Quaternion.Euler(vector);

    [Pure]
    public static bool IsZero(this Vector2 vector)
      => vector.x.IsZero() && vector.y.IsZero();

    [Pure]
    public static bool IsZero(this Vector3 vector)
      => ((Vector2)vector).IsZero() && vector.z.IsZero();

    [Pure]
    public static bool IsZero(this Vector4 vector)
      => ((Vector3)vector).IsZero() && vector.w.IsZero();

    [Pure]
    public static Vector2 Abs(this Vector2 vector)
      => vector.TransformAll(v => v.Abs());

    [Pure]
    public static Vector3 Abs(this Vector3 vector)
      => vector.TransformAll(v => v.Abs());

    [Pure]
    public static Vector2 Set(this Vector2 vector, float? xyz = null, float? x = null, float? y = null)
      => new Vector2(
        x ?? xyz ?? vector.x,
        y ?? xyz ?? vector.y);

    [Pure]
    public static Vector3 Set(this Vector3 vector, float? xyz = null, float? x = null, float? y = null, float? z = null)
      => new Vector3(
        x ?? xyz ?? vector.x,
        y ?? xyz ?? vector.y,
        z ?? xyz ?? vector.z);

    [Pure]
    public static Vector2 Add(this Vector2 vector, float xy = 0f, float x = 0f, float y = 0f)
      => vector.Add(new Vector2(x + xy, y + xy));

    [Pure]
    public static Vector3 Add(this Vector3 vector, float xyz = 0f, float x = 0f, float y = 0f, float z = 0f)
      => vector.Add(new Vector3(x + xyz, y + xyz, z + xyz));

    [Pure]
    public static Vector2 Add(this Vector2 a, Vector2 b)
      => a + b;

    [Pure]
    public static Vector3 Add(this Vector3 a, Vector3 b)
      => a + b;

    [Pure]
    public static Vector2 Subtract(this Vector2 vector, float xy = 0f, float x = 0f, float y = 0f)
      => vector.Subtract(new Vector2(x - xy, y - xy));

    [Pure]
    public static Vector3 Subtract(this Vector3 vector, float xyz = 0f, float x = 0f, float y = 0f, float z = 0f)
      => vector.Subtract(new Vector3(x - xyz, y - xyz, z - xyz));

    [Pure]
    public static Vector2 Subtract(this Vector2 a, Vector2 b)
      => a - b;

    [Pure]
    public static Vector3 Subtract(this Vector3 a, Vector3 b)
      => a - b;

    [Pure]
    public static Vector2 Multiply(this Vector2 vector, float xy = 1f, float x = 1f, float y = 1f)
      => vector.Multiply(new Vector2(x, y) / xy);

    [Pure]
    public static Vector3 Multiply(this Vector3 vector, float xyz = 1f, float x = 1f, float y = 1f, float z = 1f)
      => vector.Multiply(new Vector3(x, y, z) / xyz);

    [Pure]
    public static Vector2 Multiply(this Vector2 a, Vector2 b)
      => new Vector2(a.x * b.x, a.y * b.y);

    [Pure]
    public static Vector3 Multiply(this Vector3 a, Vector3 b)
      => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

    [Pure]
    public static Vector2 Divide(this Vector2 vector, float xy = 1f, float x = 1f, float y = 1f)
      => vector.Divide(new Vector2(x, y) / xy);

    [Pure]
    public static Vector2 Divide(this Vector3 vector, float xyz = 1f, float x = 1f, float y = 1f, float z = 1f)
      => vector.Divide(new Vector3(x, y, z) / xyz);

    [Pure]
    public static Vector2 Divide(this Vector2 a, Vector2 b)
      => new Vector2(a.x / b.x, a.y / b.y);

    [Pure]
    public static Vector3 Divide(this Vector3 a, Vector3 b)
      => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

    [Pure]
    public static Vector2 Transform(
      this Vector2 vector,
      [InstantHandle] Func<float, float> x = null,
      [InstantHandle] Func<float, float> y = null)
      => new Vector2(
        x?.Invoke(vector.x) ?? vector.x,
        y?.Invoke(vector.y) ?? vector.y);

    [Pure]
    public static Vector3 Transform(
      this Vector3 vector,
      [InstantHandle] Func<float, float> x = null,
      [InstantHandle] Func<float, float> y = null,
      [InstantHandle] Func<float, float> z = null)
      => new Vector3(
        x?.Invoke(vector.x) ?? vector.x,
        y?.Invoke(vector.y) ?? vector.y,
        z?.Invoke(vector.z) ?? vector.z);

    [Pure]
    public static Vector4 Transform(
      this Vector4 vector,
      [InstantHandle] Func<float, float> x = null,
      [InstantHandle] Func<float, float> y = null,
      [InstantHandle] Func<float, float> z = null,
      [InstantHandle] Func<float, float> w = null)
      => new Vector4(
        x?.Invoke(vector.x) ?? vector.x,
        y?.Invoke(vector.y) ?? vector.y,
        z?.Invoke(vector.z) ?? vector.z,
        z?.Invoke(vector.w) ?? vector.w);

    [Pure]
    public static Vector2 TransformAll(this Vector2 vector, [InstantHandle] Func<float, float> transform)
      => vector.Transform(transform, transform);

    [Pure]
    public static Vector3 TransformAll(this Vector3 vector, [InstantHandle] Func<float, float> transform)
      => vector.Transform(transform, transform, transform);

    [Pure]
    public static Vector4 TransformAll(this Vector4 vector, [InstantHandle] Func<float, float> transform)
      => vector.Transform(transform, transform, transform, transform);

    [Pure]
    public static float DistanceTo(this Vector3 vector, Transform transform)
      => vector.DistanceTo(transform.position);

    [Pure]
    public static float DistanceTo(this Vector2 a, Vector2 b)
      => Vector2.Distance(a, b);

    [Pure]
    public static float DistanceTo(this Vector3 a, Vector3 b)
      => Vector3.Distance(a, b);

    [Pure]
    public static Vector2 RelationTo(this Vector2 a, Vector2 b)
      => new Vector2(
        a.x >= b.x ? 1f : -1f,
        a.y >= b.y ? 1f : -1f);

    [Pure]
    public static Vector3 RelationTo(this Vector3 a, Vector3 b)
      => new Vector3(
        a.x >= b.x ? 1f : -1f,
        a.y >= b.y ? 1f : -1f,
        a.z >= b.z ? 1f : -1f);

    [Pure]
    public static Vector2 LerpTo(this Vector2 a, Vector2 b, float t)
      => Vector2.Lerp(a, b, t);

    [Pure]
    public static float Angle(this Vector3 vector)
      => Vector3.Angle(Vector3.up, vector);

    [Pure]
    public static float AngleTo(this Vector2 from, Vector2 to)
      => Vector2.Angle(from, to);

    [Pure]
    public static float AngleTo(this Vector3 from, Vector3 to)
      => Vector3.Angle(from, to);

    [Pure]
    public static Vector2 Vary(this Vector2 vector, float variance)
      => new Vector2(
        vector.x.Vary(variance),
        vector.y.Vary(variance));

    [Pure]
    public static Vector3 Vary(this Vector3 vector, float variance, bool varyZ = false)
      => new Vector3(
        vector.x.Vary(variance),
        vector.y.Vary(variance),
        varyZ ? vector.z.Vary(variance) : vector.z);

    [Pure]
    public static float RandomRange(this Vector2 parent)
      => Random.Range(parent.x, parent.y);

    /// <summary>
    /// Determines if a rotation is between 90 and 270 degrees on the z-axis.
    /// </summary>
    public static bool IsFlippedOnZAxis(this Quaternion rotation)
      => rotation.z.Abs() >= MathHelper.Cos45Deg;

    /// <summary>
    /// Converts a movement vector to a rotation that faces the movement direction.
    /// </summary>
    /// <remarks>
    /// The rotation is relative to <see cref="Vector3.forward"/>.
    /// </remarks>
    /// <example>
    /// A movement vector of 1,-1,0 results in a 45deg rotation.
    /// </example>
    [Pure]
    public static Quaternion DirectionToRotation2D(this Vector2 vector)
    {
      var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

      return Quaternion.AngleAxis(angle, Vector3.forward);
    }
  }

  public static class VectorEnumerableExtensions
  {
    [Pure]
    public static Vector2 Sum([NotNull, InstantHandle] this IEnumerable<Vector2> vectors)
      => vectors.Aggregate(Vector2.zero, (c, v) => c + v);

    [Pure]
    public static Vector3 Sum([NotNull, InstantHandle] this IEnumerable<Vector3> vectors)
      => vectors.Aggregate(Vector3.zero, (c, v) => c + v);

    [Pure]
    public static Vector2 Average([NotNull, InstantHandle] this IEnumerable<Vector2> vectors)
    {
      var vectorArray = vectors as Vector2[] ?? vectors.ToArray();

      return vectorArray.Sum() / vectorArray.Length;
    }

    [Pure]
    public static Vector3 Average([NotNull, InstantHandle] this IEnumerable<Vector3> vectors)
    {
      var vectorArray = vectors as Vector3[] ?? vectors.ToArray();

      return vectorArray.Sum() / vectorArray.Length;
    }
  }
}