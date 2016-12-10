﻿using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace PachowStudios.Framework.Primitives
{
  public static class Wait
  {
    private static readonly Lazy<WaitInternal> instance = Lazy.From(CreateInstance);

    private static WaitInternal Instance => instance.Value;

    public static void ForSeconds(float waitTime, [NotNull] Action callback)
      => Instance.StartCoroutine(WaitInternal.ForSecondsCoroutine(waitTime, callback));

    public static void ForRealSeconds(float waitTime, [NotNull] Action callback)
      => Instance.StartCoroutine(WaitInternal.ForRealSecondsCoroutine(waitTime, callback));

    public static void ForFixedUpdate([NotNull] Action callback)
      => Instance.StartCoroutine(WaitInternal.ForFixedUpdateCoroutine(callback));

    public static void ForEndOfFrame([NotNull] Action callback)
      => Instance.StartCoroutine(WaitInternal.ForEndOfFrameCoroutine(callback));

    private static WaitInternal CreateInstance()
    {
      var newInstance = new GameObject("Wait Utility");

      newInstance.HideInHierarchy();

      return newInstance.AddComponent<WaitInternal>();
    }

    private class WaitInternal : MonoBehaviour
    {
      public static IEnumerator ForSecondsCoroutine(float waitTime, Action callback)
      {
        yield return new WaitForSeconds(waitTime);

        callback?.Invoke();
      }

      public static IEnumerator ForRealSecondsCoroutine(float waitTime, Action callback)
      {
        var startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < startTime + waitTime)
          yield return null;

        callback?.Invoke();
      }

      public static IEnumerator ForFixedUpdateCoroutine(Action callback)
      {
        yield return new WaitForFixedUpdate();

        callback?.Invoke();
      }

      public static IEnumerator ForEndOfFrameCoroutine(Action callback)
      {
        yield return new WaitForEndOfFrame();

        callback?.Invoke();
      }
    }
  }
}