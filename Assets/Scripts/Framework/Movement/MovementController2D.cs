#define DEBUG_MC2D_RAYS
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
  public partial class MovementController2D
  {
    public event Action<RaycastHit2D> Collided;
    public event Action<RaycastHit2D> Triggered;

    [SerializeField] protected LayerMask collisionMask = 0;
    [SerializeField] protected LayerMask triggerMask = 0;
    [SerializeField] protected float stopThreshold = 0.001f;

    [SerializeField, Range(0.001f, 0.3f)] protected float skinWidth = 0.02f;
    [SerializeField, Range(2, 20)] protected int horizontalRays = 4;
    [SerializeField, Range(2, 20)] protected int verticalRays = 4;
    [SerializeField] protected bool raiseAllCollidedEvents = false;
    [SerializeField] protected bool raiseAllTriggerEvents = false;

    private Transform transformComponent;
    private BoxCollider2D boxColliderComponent;

    private static LambdaEqualityComparer<RaycastHit2D, Collider2D> RaycastComparer { get; }
      = LambdaEqualityComparer<RaycastHit2D>.Create(r => r.collider);

    protected HashSet<RaycastHit2D> CollisionsThisFrame { get; } = new HashSet<RaycastHit2D>(RaycastComparer);
    protected HashSet<RaycastHit2D> TriggersThisFrame { get; } = new HashSet<RaycastHit2D>(RaycastComparer);

    protected CollisionState CurrentCollisionState { get; set; } = new CollisionState();
    protected Origin RaycastOrigin { get; set; }
    protected float VerticalDistanceBetweenRays { get; set; }
    protected float HorizontalDistanceBetweenRays { get; set; }

    protected Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
    protected BoxCollider2D BoxCollider => this.GetComponentIfNull(ref this.boxColliderComponent);

    protected bool FireTriggerRaycasts => this.triggerMask != 0;

    private static void RaiseRaycastEvents(Action<RaycastHit2D> @event, IEnumerable<RaycastHit2D> raycasts, bool raiseAll)
    {
      if (@event == null)
        return;

      if (!raiseAll)
        raycasts = raycasts.Take(1);

      raycasts.ForEach(@event);
    }

    [Conditional("DEBUG_MC2D_RAYS")]
    protected static void DrawRay(Vector3 start, Vector3 direction, Color color)
      => Debug.DrawRay(start, direction, color);

    protected virtual void Awake() => CalculateDistanceBetweenRays();

    public virtual Vector2 Move(Vector2 movement)
    {
      if (Time.deltaTime <= 0f || Time.timeScale <= 0.01f)
        return movement;

      movement *= Time.deltaTime;

      ResetCollisionState();
      RaycastOrigin = CalculateRaycastOrigin();
      CollisionsThisFrame.Clear();
      TriggersThisFrame.Clear();

      if (movement.x.IsAboveThreshold(MathHelper.FloatingPointTolerance))
        movement = MoveHorizontally(movement);

      if (movement.y.IsAboveThreshold(MathHelper.FloatingPointTolerance))
        movement = MoveVertically(movement);

      Transform.Translate(movement, Space.World);
      RaiseRaycastEvents();

      movement /= Time.deltaTime;

      if (movement.x.IsUnderThreshold(this.stopThreshold))
        movement.x = 0f;

      if (movement.y.IsUnderThreshold(this.stopThreshold))
        movement.y = 0f;

      return movement;
    }

    protected virtual void ResetCollisionState() => CurrentCollisionState = new CollisionState();

    protected virtual Vector2 MoveHorizontally(Vector2 movement)
    {
      var isMovingRight = movement.x > 0f;
      var rayDistance = movement.x.Abs() + this.skinWidth;
      var rayDirection = isMovingRight ? Vector2.right : Vector2.left;
      var initialRayOrigin = isMovingRight ? RaycastOrigin.BottomRight : RaycastOrigin.BottomLeft;

      for (var i = 0; i < this.horizontalRays; i++)
      {
        var rayOrigin = initialRayOrigin.Add(y: i * VerticalDistanceBetweenRays);

        DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (FireTriggerRaycasts)
          RaycastTriggers(rayOrigin, rayDirection, rayDistance);

        var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, this.collisionMask);

        if (!raycast)
          continue;

        CollisionsThisFrame.Add(raycast);

        movement.x = raycast.point.x - rayOrigin.x;
        rayDistance = movement.x.Abs();

        if (isMovingRight)
        {
          movement.x -= this.skinWidth;
          CurrentCollisionState.Right = true;
        }
        else
        {
          movement.x += this.skinWidth;
          CurrentCollisionState.Left = true;
        }

        if (rayDistance < this.skinWidth + MathHelper.FloatingPointTolerance)
          break;
      }

      return movement;
    }

    protected virtual Vector2 MoveVertically(Vector2 movement)
    {
      var isMovingUp = movement.y > 0;
      var rayDistance = movement.y.Abs() + this.skinWidth;
      var rayDirection = isMovingUp ? Vector2.up : Vector2.down;
      var initialRayOrigin = isMovingUp ? RaycastOrigin.TopLeft : RaycastOrigin.BottomLeft;

      initialRayOrigin.x += movement.x;

      for (var i = 0; i < this.verticalRays; i++)
      {
        var rayOrigin = initialRayOrigin.Add(x: i * HorizontalDistanceBetweenRays);

        DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (FireTriggerRaycasts)
          RaycastTriggers(rayOrigin, rayDirection, rayDistance);

        var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, this.collisionMask);

        if (!raycast)
          continue;

        CollisionsThisFrame.Add(raycast);

        movement.y = raycast.point.y - rayOrigin.y;
        rayDistance = movement.y.Abs();

        if (isMovingUp)
        {
          movement.y -= this.skinWidth;
          CurrentCollisionState.Up = true;
        }
        else
        {
          movement.y += this.skinWidth;
          CurrentCollisionState.Down = true;
        }

        if (rayDistance < this.skinWidth + MathHelper.FloatingPointTolerance)
          break;
      }

      return movement;
    }

    protected Origin CalculateRaycastOrigin()
    {
      var bounds = BoxCollider.bounds;

      bounds.Expand(-2f * this.skinWidth);

      return new Origin(bounds);
    }

    protected void RaycastTriggers(Vector3 rayOrigin, Vector2 rayDirection, float rayDistance)
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

    private void CalculateDistanceBetweenRays()
    {
      var absScale = Transform.localScale.Abs();
      var totalSkinWidth = 2f * this.skinWidth;
      var useableHeight = BoxCollider.size.y * absScale.y - totalSkinWidth;
      var useableWidth = BoxCollider.size.x * absScale.x - totalSkinWidth;

      VerticalDistanceBetweenRays = useableHeight / (this.horizontalRays - 1);
      HorizontalDistanceBetweenRays = useableWidth / (this.verticalRays - 1);
    }
  }
}
