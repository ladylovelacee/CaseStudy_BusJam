using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Core
{
    public static class WaitExtension
    {
        public static void Wait(this MonoBehaviour mono, float delay, UnityAction action)
        {
            mono.StartCoroutine(ExecuteAction(delay, action));
        }
        
        public static void Wait(this MonoBehaviour mono, float delay, Action action)
        {
            mono.StartCoroutine(ExecuteAction(delay, action));
        }

        static IEnumerator ExecuteAction(float delay, UnityAction action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action.Invoke();
            yield break;
        }
        
        static IEnumerator ExecuteAction(float delay, Action action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
            yield break;
        }
    }
}