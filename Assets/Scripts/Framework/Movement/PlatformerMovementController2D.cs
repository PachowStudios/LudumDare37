using UnityEngine;

namespace PachowStudios.Framework.Movement
{
  [AddComponentMenu("Pachow Studios/Movement/Platformer Movement Controller 2D")]
  public class PlatformerMovementController2D : MovementController2D
  {
    // We require a more precise floating point tolerance than normal.
    private const float FloatingPointTolerance = 0.000001f;
    private const float SlopeLimitTangent = 3.73205f;

    [SerializeField] private LayerMask oneWayPlatformMask = 0;
    [SerializeField] private float jumpThreshold = 0.07f;
    [SerializeField] private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(
      new Keyframe(-90, 1.5f),
      new Keyframe(0, 1),
      new Keyframe(90, 0));
    [SerializeField, Range(0, 90f)] private float slopeLimit = 30f;

    private bool IsGoingUpSlope { get; set; }

    public Vector3 CenterPoint => BoxCollider.bounds.center;
    public bool IsGrounded => CurrentCollisionState.Down;
    public bool WasGroundedLastFrame { get; set; }
    public bool IsColliding => CurrentCollisionState.Any;

    protected override void Awake()
    {
      this.collisionMask |= this.oneWayPlatformMask;
      base.Awake();
    }

    public override Vector2 Move(Vector2 movement)
    {
      movement = base.Move(movement);

      if (IsGoingUpSlope)
        movement.y = 0f;

      return movement;
    }

    protected override void ResetCollisionState()
    {
      WasGroundedLastFrame = CurrentCollisionState.Down;
      IsGoingUpSlope = false;
      base.ResetCollisionState();
    }

    protected override Vector2 MoveHorizontally(Vector2 movement)
    {
      if (movement.y < 0f && WasGroundedLastFrame)
        movement = HandleVerticalSlope(movement);

      var isMovingRight = movement.x > 0;
      var rayDistance = movement.x.Abs() + this.skinWidth;
      var rayDirection = isMovingRight ? Vector2.right : Vector2.left;
      var initialRayOrigin = isMovingRight ? RaycastOrigin.BottomRight : RaycastOrigin.BottomLeft;

      for (var i = 0; i < this.horizontalRays; i++)
      {
        var rayOrigin = initialRayOrigin.Add(y: i * VerticalDistanceBetweenRays);

        DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (FireTriggerRaycasts)
          RaycastTriggers(rayOrigin, rayDirection, rayDistance);

        var colliderMask = i == 0 && WasGroundedLastFrame
          ? (int)this.collisionMask
          : this.collisionMask & ~this.oneWayPlatformMask;

        var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, colliderMask);

        if (!raycast)
          continue;

        CollisionsThisFrame.Add(raycast);

        // The bottom ray can hit slopes but no other ray can so we have special handling for those cases
        if (i == 0)
        {
          bool isOnSlope;
          movement = HandleHorizontalSlope(movement, raycast.normal.AngleTo(Vector2.up), out isOnSlope);

          if (isOnSlope)
            break;
        }

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

        if (rayDistance < this.skinWidth + 0.001f)
          break;
      }

      return movement;
    }

    protected override Vector2 MoveVertically(Vector2 movement)
    {
      var isMovingUp = movement.y > 0;
      var rayDistance = movement.y.Abs() + this.skinWidth;
      var rayDirection = isMovingUp ? Vector2.up : Vector2.down;
      var initialRayOrigin = isMovingUp ? RaycastOrigin.TopLeft : RaycastOrigin.BottomLeft;
      var mask = this.collisionMask;

      initialRayOrigin.x += movement.x;

      if (isMovingUp && !WasGroundedLastFrame)
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

        if (!isMovingUp && movement.y > 0.00001f)
          IsGoingUpSlope = true;

        if (rayDistance < this.skinWidth + 0.001f)
          break;
      }

      return movement;
    }

    private Vector2 HandleHorizontalSlope(Vector2 movement, float angle, out bool isOnSlope)
    {
      isOnSlope = false;

      if (angle.RoundToInt() == 90)
        return movement;

      isOnSlope = true;

      if (angle >= this.slopeLimit)
      {
        movement.x = 0;
        return movement;
      }

      if (movement.y >= this.jumpThreshold)
        return movement;

      movement.x *= this.slopeSpeedMultiplier.Evaluate(angle);
      movement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * movement.x);

      IsGoingUpSlope = true;
      CurrentCollisionState.Down = true;

      return movement;
    }

    private Vector2 HandleVerticalSlope(Vector2 movement)
    {
      var colliderCenter = (RaycastOrigin.BottomLeft.x + RaycastOrigin.BottomRight.x) / 2f;
      var rayOrigin = new Vector2(colliderCenter, RaycastOrigin.BottomLeft.y);
      var rayDirection = Vector2.down;
      var rayDistance = SlopeLimitTangent * (RaycastOrigin.BottomRight.x - colliderCenter);

      DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);

      var raycast = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, this.collisionMask);

      if (!raycast)
        return movement;

      var angle = Vector2.Angle(raycast.normal, Vector2.up);

      if (angle.IsUnderThreshold(FloatingPointTolerance))
        return movement;

      var isMovingDownSlope = raycast.normal.x.Sign() == movement.x.Sign();

      if (!isMovingDownSlope)
        return movement;

      movement.x *= this.slopeSpeedMultiplier.Evaluate(-angle);
      movement.y = raycast.point.y - rayOrigin.y - this.skinWidth;

      return movement;
    }
  }
}
