using System;
using System.Collections;
using UnityEngine;

namespace Runtime.Core
{
    public static class WaitExtension
    {
        public static void Wait(this MonoBehaviour mono, float delay, Action action)
        {
            mono.StartCoroutine(ExecuteAction(delay, action));
        }

        static IEnumerator ExecuteAction(float delay, Action action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
            yield break;
        }
    }
}