#define DEBUG_CC2D_RAYS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Extensions;
using PachowStudios.Framework.Primitives;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PachowStudios.Framework.Movement
{
  [AddComponentMenu("Pachow Studios/Movement/Movement Controller 2D")]
  [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
  public partial class MovementController2D : MonoBehaviour, IRaycastSource
  {
    // We require a more precise floating point tolerance than normal.
    private const float FloatingPointTolerance = 0.000001f;
    private const float SlopeLimitTangent = 3.73205f;

    public event Action<RaycastHit2D> Collided;
    public event Action<RaycastHit2D> Triggered;

    [SerializeField] private LayerMask platformMask = 0;
    [SerializeField] private LayerMask oneWayPlatformMask = 0;
    [SerializeField] private LayerMask triggerMask = 0;
    [SerializeField] private float stopThreshold = 0.001f;
    [SerializeField] private float jumpThreshold = 0.07f;
    [SerializeField] private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(
      new Keyframe(-90, 1.5f),
      new Keyframe(0, 1),
      new Keyframe(90, 0));
    [SerializeField, Range(0, 90f)] private float slopeLimit = 30f;
    [SerializeField, Range(0.001f, 0.3f)] private float skinWidth = 0.02f;
    [SerializeField, Range(2, 20)] private int horizontalRays = 8;
    [SerializeField, Range(2, 20)] private int verticalRays = 4;
    [SerializeField] private bool raiseAllCollidedEvents = false;
    [SerializeField] private bool raiseAllTriggerEvents = false;

    private Transform transformComponent;
    private BoxCollider2D boxColliderComponent;

    private static LambdaEqualityComparer<RaycastHit2D, Collider2D> RaycastComparer { get; } = LambdaEqualityComparer<RaycastHit2D>.Create(r => r.collider);

    private Origin RaycastOrigin { get; set; }
    private float VerticalDistanceBetweenRays { get; set; }
    private float HorizontalDistanceBetweenRays { get; set; }
    private bool IsGoingUpSlope { get; set; }

    private HashSet<RaycastHit2D> CollisionsThisFrame { get; } = new HashSet<RaycastHit2D>(RaycastComparer);
    private HashSet<RaycastHit2D> TriggersThisFrame { get; } = new HashSet<RaycastHit2D>(RaycastComparer);
    private CollisionState CurrentCollisionState { get; } = new CollisionState();

    public Vector3 CenterPoint => BoxCollider.bounds.center;
    public bool IsGrounded => CurrentCollisionState.Below;
    public bool WasGroundedLastFrame => CurrentCollisionState.WasGroundedLastFrame;
    public bool IsColliding => CurrentCollisionState.Any;
    public LayerMask PlatformMask => this.platformMask;

    private Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
    private BoxCollider2D BoxCollider => this.GetComponentIfNull(ref this.boxColliderComponent);

    private bool FireTriggerRaycasts => this.triggerMask != 0;

    private void Awake()
    {
      this.platformMask |= this.oneWayPlatformMask;
      RecalculateDistanceBetweenRays();
    }

    public Vector2 Move(Vector2 deltaMovement)
    {
      if (Time.deltaTime <= 0f || Time.timeScale <= 0.01f)
        return deltaMovement;

      deltaMovement *= Time.deltaTime;

      CurrentCollisionState.WasGroundedLastFrame = CurrentCollisionState.Below;
      CurrentCollisionState.Reset();
      CollisionsThisFrame.Clear();
      TriggersThisFrame.Clear();
      IsGoingUpSlope = false;

      RecalculateRaycastOrigin();

      if (deltaMovement.y < 0f && CurrentCollisionState.WasGroundedLastFrame)
        HandleVerticalSlope(ref deltaMovement);

      if (deltaMovement.x.IsAboveThreshold(FloatingPointTolerance))
        MoveHorizontally(ref deltaMovement);

      if (deltaMovement.y.IsAboveThreshold(FloatingPointTolerance))
        MoveVertically(ref deltaMovement);

      Transform.Translate(deltaMovement, Space.World);

      deltaMovement /= Time.deltaTime;

      if (!CurrentCollisionState.WasGroundedLastFrame && CurrentCollisionState.Below)
        CurrentCollisionState.BecameGroundedThisFrame = true;

      if (IsGoingUpSlope)
        deltaMovement.y = 0f;

      RaiseRaycastEvents();

      if (deltaMovement.x.IsUnderThreshold(this.stopThreshold))
        deltaMovement.x = 0f;

      if (deltaMovement.y.IsUnderThreshold(this.stopThreshold))
        deltaMovement.y = 0f;

      return deltaMovement;
    }

    private static void RaiseRaycastEvents(Action<RaycastHit2D> @event, IEnumerable<RaycastHit2D> raycasts, bool raiseAll)
    {
      if (@event == null)
        return;

      if (!raiseAll)
        raycasts = raycasts.Take(1);

      raycasts.ForEach(@event);
    }

    [Conditional("DEBUG_CC2D_RAYS")]
    private static void DrawRay(Vector3 start, Vector3 direction, Color color)
      => Debug.DrawRay(start, direction, color);

    private void RecalculateDistanceBetweenRays()
    {
      var absScale = Transform.localScale.Abs();
      var totalSkinWidth = 2f * this.skinWidth;
      var useableHeight = (BoxCollider.size.y * absScale.y) - totalSkinWidth;
      var useableWidth = (BoxCollider.size.x * absScale.x) - totalSkinWidth;

      VerticalDistanceBetweenRays = useableHeight / (this.horizontalRays - 1);
      HorizontalDistanceBetweenRays = useableWidth / (this.verticalRays - 1);
    }

    private void RecalculateRaycastOrigin()
    {
      var bounds = BoxCollider.bounds;

      bounds.Expand(-this.skinWidth);

      RaycastOrigin = new Origin(
        new Vector2(bounds.min.x, bounds.max.y),
        new Vector2(bounds.max.x, bounds.min.y),
        bounds.min);
    }

    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
      var isMovingRight = deltaMovement.x > 0;
      var rayDistance = deltaMovement.x.Abs() + this.skinWidth;
      var rayDirection = isMovingRight ? Vector2.right : Vector2.left;
      var initialRayOrigin = isMovingRight ? RaycastOrigin.BottomRight : RaycastOrigin.BottomLeft;

      for (var i = 0; i < this.horizontalRays; i++)
      {
        var rayOrigin = initialRayOrigin.Add(y: i * VerticalDistanceBetweenRays);

        DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (FireTriggerRaycasts)
          RaycastTriggers(rayOrigin, rayDirection, rayDistance);

        var colliderMask = i == 0 && CurrentCollisionState.WasGroundedLastFrame
          ? (int)PlatformMask
          : PlatformMask & ~this.oneWayPlatformMask;

        var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, colliderMask);

        if (!raycast)
          continue;

        CollisionsThisFrame.Add(raycast);

        // The bottom ray can hit slopes but no other ray can so we have special handling for those cases
        if (i == 0 && HandleHorizontalSlope(ref deltaMovement, raycast.normal.AngleTo(Vector2.up)))
          break;

        deltaMovement.x = raycast.point.x - rayOrigin.x;
        rayDistance = deltaMovement.x.Abs();

        if (isMovingRight)
        {
          deltaMovement.x -= this.skinWidth;
          CurrentCollisionState.Right = true;
        }
        else
        {
          deltaMovement.x += this.skinWidth;
          CurrentCollisionState.Left = true;
        }

        if (rayDistance < this.skinWidth + 0.001f)
          break;
      }
    }

    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle)
    {
      if (angle.RoundToInt() == 90)
        return false;

      if (angle >= this.slopeLimit)
      {
        deltaMovement.x = 0;
        return true;
      }

      if (deltaMovement.y >= this.jumpThreshold)
        return true;

      deltaMovement.x *= this.slopeSpeedMultiplier.Evaluate(angle);
      deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);

      IsGoingUpSlope = true;
      CurrentCollisionState.Below = true;

      return true;
    }

    private void MoveVertically(ref Vector2 deltaMovement)
    {
      var isMovingUp = deltaMovement.y > 0;
      var rayDistance = deltaMovement.y.Abs() + this.skinWidth;
      var rayDirection = isMovingUp ? Vector2.up : Vector2.down;
      var initialRayOrigin = isMovingUp ? RaycastOrigin.TopLeft : RaycastOrigin.BottomLeft;
      var mask = PlatformMask;

      initialRayOrigin.x += deltaMovement.x;

      if (isMovingUp && !CurrentCollisionState.WasGroundedLastFrame)
        mask &= ~this.oneWayPlatformMask;

      for (var i = 0; i < this.verticalRays; i++)
      {
        var rayOrigin = initialRayOrigin.Add(x: i * HorizontalDistanceBetweenRays);

        DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (FireTriggerRaycasts)
          RaycastTriggers(rayOrigin, rayDirection, rayDistance);

        var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, mask);

        if (!raycast)
          continue;

        CollisionsThisFrame.Add(raycast);

        deltaMovement.y = raycast.point.y - rayOrigin.y;
        rayDistance = deltaMovement.y.Abs();

        if (isMovingUp)
        {
          deltaMovement.y -= this.skinWidth;
          CurrentCollisionState.Above = true;
        }
        else
        {
          deltaMovement.y += this.skinWidth;
          CurrentCollisionState.Below = true;
        }

        if (!isMovingUp && deltaMovement.y > 0.00001f)
          IsGoingUpSlope = true;

        if (rayDistance < this.skinWidth + 0.001f)
          return;
      }
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
      var colliderCenter = (RaycastOrigin.BottomLeft.x + RaycastOrigin.BottomRight.x) / 2f;
      var rayOrigin = new Vector2(colliderCenter, RaycastOrigin.BottomLeft.y);
      var rayDirection = Vector2.down;
      var rayDistance = SlopeLimitTangent * (RaycastOrigin.BottomRight.x - colliderCenter);

      DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);

      var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);

      if (!raycast)
        return;

      var angle = Vector2.Angle(raycast.normal, Vector2.up);

      if (angle.IsUnderThreshold(FloatingPointTolerance))
        return;

      var isMovingDownSlope = raycast.normal.x.Sign() == deltaMovement.x.Sign();

      if (!isMovingDownSlope)
        return;

      deltaMovement.x *= this.slopeSpeedMultiplier.Evaluate(-angle);
      deltaMovement.y = raycast.point.y - rayOrigin.y - this.skinWidth;

      CurrentCollisionState.IsMovingDownSlope = true;
      CurrentCollisionState.SlopeAngle = angle;
    }

    private void RaycastTriggers(Vector3 rayOrigin, Vector2 rayDirection, float rayDistance)
    {
      var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, this.triggerMask);

      if (raycast)
        TriggersThisFrame.Add(raycast);
    }

    private void RaiseRaycastEvents()
    {
      RaiseRaycastEvents(Collided, CollisionsThisFrame, this.raiseAllCollidedEvents);
      RaiseRaycastEvents(Triggered, TriggersThisFrame, this.raiseAllTriggerEvents);
    }
  }
}
