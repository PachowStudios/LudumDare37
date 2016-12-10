﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Extensions;
using PachowStudios.Framework.Collections;
using PachowStudios.Framework.Movement;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace PachowStudios.Framework.Camera
{
  [AddComponentMenu("Pachow Studios/Camera/Camera Controller")]
  [RequireComponent(typeof(UnityCamera))]
  public class CameraController : MonoBehaviour
  {
    [SerializeField] private Collider2D targetCollider = null;
    [Tooltip("percentage from -0.5 - 0.5 from the center of the screen")]
    [SerializeField, Range(-0.5f, 0.5f)] private float horizontalOffset = 0f;
    [Tooltip("percentage from -0.5 - 0.5 from the center of the screen")]
    [SerializeField, Range(-0.5f, 0.5f)] private float verticalOffset = 0f;

    [Header("Platform Snap")]
    [Tooltip("All platform snap settings only apply if enabled")]
    [SerializeField] private bool enablePlatformSnap = false;
    [Tooltip("If true, no other base behaviours will be able to modify the y-position of the camera when grounded")]
    [SerializeField] private bool isPlatformSnapExclusiveWhenEnabled = false;
    [SerializeField, Range(-10f, 10f)] private float platformSnapVerticalOffset = 0f;

    [Header("Smoothing")]
    [SerializeField] private CameraSmoothingType cameraSmoothingType = CameraSmoothingType.SmoothDamp;
    [Tooltip("Approximately the time it will take to reach the target. A smaller value will reach the target faster.")]
    [SerializeField] private float smoothDampTime = 0.08f;
    [Tooltip("Lower values are less damped and higher values are more damped resulting in less springiness. should be between 0.01f, 1f to avoid unstable systems.")]
    [SerializeField] private float springDampingRatio = 0.7f;
    [Tooltip("An angular frequency of 2pi (radians per second) means the oscillation completes one full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable")]
    [SerializeField] private float springAngularFrequency = 20f;
    [SerializeField] private float lerpTowardsFactor = 0.002f;

    private readonly List<ICameraBehaviour> behaviours = new List<ICameraBehaviour>(3);
    private readonly List<ICameraEffector> effectors = new List<ICameraEffector>(3);
    private readonly List<ICameraFinalizer> finalizers = new List<ICameraFinalizer>(1);

    private readonly FixedQueue<Vector3> averageVelocityQueue = new FixedQueue<Vector3>(10);
    private Vector3 targetPositionLastFrame;
    private Vector3 cameraVelocity;

    private UnityCamera controlledCamera;
    private IGroundable groundableTargetComponent;

    public UnityCamera Camera => this.GetComponentIfNull(ref this.controlledCamera);

    private IGroundable GroundableTarget => this.GetComponentIfNull(ref this.groundableTargetComponent);
    private bool IsTargetGrounded => GroundableTarget?.IsGrounded ?? false;

    private void Awake()
    {
      GetComponents<ICameraBehaviour>().ForEach(AddBehavior);
      GetComponents<ICameraFinalizer>().ForEach(AddFinalizer);
    }

    private void LateUpdate()
    {
      var targetBounds = this.targetCollider.bounds;

      // we keep track of the target's velocity since some camera behaviours need to know about it
      var velocity = (targetBounds.center - this.targetPositionLastFrame) / Time.deltaTime;
      velocity.z = 0f;
      this.averageVelocityQueue.Enqueue(velocity);
      this.targetPositionLastFrame = targetBounds.center;

      // fetch the average velocity for use in our camera behaviours
      var targetAvgVelocity = this.averageVelocityQueue.Average();

      // we use the transform.position plus the offset when passing the base position to our camera behaviours
      var basePosition = GetNormalizedCameraPosition();
      var accumulatedDeltaOffset = this.behaviours
        .Where(b => b.IsEnabled)
        .Select(b => b.GetDesiredPositionDelta(targetBounds, basePosition, targetAvgVelocity))
        .Aggregate(Vector3.zero, (current, desired) => current + desired);

      if (this.enablePlatformSnap && IsTargetGrounded)
      {
        // when exclusive, no base behaviours can mess with y
        if (this.isPlatformSnapExclusiveWhenEnabled)
          accumulatedDeltaOffset.y = 0f;

        var desiredOffset = targetBounds.min.y - basePosition.y - this.platformSnapVerticalOffset;

        accumulatedDeltaOffset += new Vector3(0f, desiredOffset);
      }

      // fetch our effectors
      var totalWeight = 0f;
      var accumulatedEffectorPosition = Vector3.zero;

      foreach (var effector in this.effectors)
      {
        var weight = effector.GetEffectorWeight();
        var position = effector.GetDesiredPositionDelta(targetBounds, basePosition, targetAvgVelocity);

        totalWeight += weight;
        accumulatedEffectorPosition += weight * position;
      }

      var desiredPosition = transform.position + accumulatedDeltaOffset;

      // if we have a totalWeight we need to take into account our effectors
      if (totalWeight > 0)
      {
        totalWeight += 1f;
        accumulatedEffectorPosition += desiredPosition;

        var finalAccumulatedPosition = accumulatedEffectorPosition / totalWeight;

        finalAccumulatedPosition.z = transform.position.z;
        desiredPosition = finalAccumulatedPosition;
      }

      var smoothing = this.cameraSmoothingType;

      // and finally, our finalizers have a go if we have any
      for (var i = 0; i < this.finalizers.Count; i++)
      {
        var finalizer = this.finalizers[i];
        desiredPosition = finalizer.GetFinalCameraPosition(targetBounds, transform.position, desiredPosition);

        // allow the finalizer with a 0 priority to skip smoothing if it wants to
        if (i == 0
            && finalizer.GetFinalizerPriority == 0
            && finalizer.ShouldSkipSmoothingThisFrame)
          smoothing = CameraSmoothingType.None;
      }

      // reset Z just in case one of the other scripts messed with it
      desiredPosition.z = transform.position.z;

      // time to smooth our movement to the desired position
      switch (smoothing)
      {
        case CameraSmoothingType.None:
          transform.position = desiredPosition;
          break;
        case CameraSmoothingType.SmoothDamp:
          transform.position =
            Vector3.SmoothDamp(
              transform.position,
              desiredPosition,
              ref this.cameraVelocity,
              this.smoothDampTime);
          break;
        case CameraSmoothingType.Spring:
          transform.position = FastSpring(transform.position, desiredPosition);
          break;
        case CameraSmoothingType.Lerp:
          transform.position = LerpTowards(transform.position, desiredPosition, this.lerpTowardsFactor);
          break;
      }
    }

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
    {
      if (Camera == null)
        return;

      var positionInFrontOfCamera = GetNormalizedCameraPosition();

      positionInFrontOfCamera.z = 1f;

      foreach (var baseBehavior in GetComponents<ICameraBehaviour>().Where(b => b.IsEnabled))
        baseBehavior.OnDrawGizmosInternal(positionInFrontOfCamera);

      foreach (var finalizer in GetComponents<ICameraFinalizer>().Where(f => f.IsEnabled))
        finalizer.OnDrawGizmosInternal(positionInFrontOfCamera);

      if (!this.enablePlatformSnap)
        return;

      Gizmos.color = new Color(0.3f, 0.1f, 0.6f);

      var lineWidth = UnityCamera.main.orthographicSize / 2f;

      Gizmos.DrawLine(
        positionInFrontOfCamera + new Vector3(-lineWidth, this.platformSnapVerticalOffset, 1f),
        positionInFrontOfCamera + new Vector3(lineWidth, this.platformSnapVerticalOffset, 1f));
    }

    public void AddBehavior(ICameraBehaviour behaviour)
      => this.behaviours.Add(behaviour);

    public void RemoveBehavior(ICameraBehaviour behaviour)
      => this.behaviours.Remove(behaviour);

    public void AddEffector(ICameraEffector effector)
      => this.effectors.Add(effector);

    public void RemoveEffector(ICameraEffector effector)
      => this.effectors.Remove(effector);

    public void AddFinalizer(ICameraFinalizer finalizer)
    {
      this.finalizers.Add(finalizer);

      if (this.finalizers.HasMultiple())
        this.finalizers.SortBy(f => f.GetFinalizerPriority);
    }

    public void RemoveFinalizer(ICameraFinalizer finalizer)
      => this.finalizers.Remove(finalizer);

    private static Vector3 LerpTowards(Vector3 from, Vector3 to, float remainingFactorPerSecond)
      => Vector3.Lerp(from, to, 1f - Mathf.Pow(remainingFactorPerSecond, Time.deltaTime));

    private Vector3 FastSpring(Vector3 currentValue, Vector3 targetValue)
    {
      this.cameraVelocity += (-2.0f * Time.deltaTime * this.springDampingRatio * this.springAngularFrequency * this.cameraVelocity)
                             + (Time.deltaTime * this.springAngularFrequency * this.springAngularFrequency * (targetValue - currentValue));
      currentValue += Time.deltaTime * this.cameraVelocity;

      return currentValue;
    }

    private Vector3 GetNormalizedCameraPosition()
      => Camera.ViewportToWorldPoint(new Vector3(0.5f + this.horizontalOffset, 0.5f + this.verticalOffset, 0f));
  }
}