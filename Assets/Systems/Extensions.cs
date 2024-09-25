using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace BSA
{
    public enum Axis
    {
        X,
        Y,
        Z,
        W
    }

    public enum Space
    {
        global = 1,
        local = 2,
    }

    public static class Extensions
    {
        // --- Fields -------------------------------------------------------------------------------------------------


        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Constructors -------------------------------------------------------------------------------------------

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public static void SetPosition(this Transform t, Axis axis, float value, Space space = Space.global)
        {
            Vector3 pos = t.position;
            //       switch(axis)
            //       {
            //           case Axis.X:
            //t.position = new Vector3(value, pos.y, pos.z);
            //               break;

            //           case Axis.Y:
            //               t.position = new Vector3(pos.x, value, pos.z);
            //               break;

            //           case Axis.Z:
            //               t.position = new Vector3(pos.x, pos.y, value);
            //               break;

            //           case Axis.W:
            //throw new NotImplementedException();
            //       }
            if(space == Space.global)
            {
                t.position = axis switch
                {
                    Axis.X => new Vector3(value, pos.y, pos.z),
                    Axis.Y => new Vector3(pos.x, value, pos.z),
                    Axis.Z => new Vector3(pos.x, pos.y, value),
                    _ => throw new NotImplementedException(),
                };
            } else if (space == Space.local)
            {
                t.localPosition = axis switch
                {
                    Axis.X => new Vector3(value, pos.y, pos.z),
                    Axis.Y => new Vector3(pos.x, value, pos.z),
                    Axis.Z => new Vector3(pos.x, pos.y, value),
                    _ => throw new NotImplementedException(),
                };
            }
        }        

        public static void DoAfter(this MonoBehaviour mb, float delay, Action action)
        {
            if(action == null)
                return;

            mb.StartCoroutine(ExecuteDelayedRoutine(delay, action));
        }

        private static IEnumerator ExecuteDelayedRoutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        public static T GetRandomElement<T>(this IEnumerable<T> collection)
        {
            if(collection == null)
                throw new ArgumentNullException("Collection is NULL");

            int count = collection.Count();
            if(count == 0)
                throw new ArgumentException("Can't pick from empty Collection.");

            int index = UnityEngine.Random.Range(0, count);
            return collection.ElementAt(index);
        }

        public static Coroutine AutoLerp(this MonoBehaviour mono, float from, float to, float duration, Action<float> assign, EasingType easing = EasingType.Linear)
        {
            return mono.StartCoroutine(AutoLerpRoutine(from, to, duration, assign, easing));
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private static IEnumerator AutoLerpRoutine(float from, float to, float duration, Action<float> assign, EasingType easing = EasingType.Linear)
        {
            double startTime = Time.timeAsDouble;
            float t = 0f;

            float value = from;
            assign(value);

            while(t < 1f)
            {
                yield return null;
                t = Mathf.Clamp01((float)(Time.timeAsDouble - startTime) / duration);
                t = Ease(t, easing);
                value = Mathf.Lerp(from, to, t);
                assign(value);
            }
        }

        private static float Ease(float t, EasingType easingType)
        {
            switch(easingType)
            {
                case EasingType.Linear:
                    break;
                case EasingType.Smooth:
                    t = t * t * (3f - 2f * t);
                    break;
                case EasingType.Smoother:
                    t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
                    break;
                case EasingType.EasyInOutSine:
                    t = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                    break;
                case EasingType.EasyOutSine:
                    t = Mathf.Sin((t * Mathf.PI) / 2);
                    break;
                case EasingType.EasyOutQuart:
                    t = 1 - Mathf.Pow(1 - t, 4);
                    break;
                case EasingType.EasyInQuart:
                    t = t* t * t * t;
                    break;
            }
            return t;
        }

        // ----------------------------------------------------------------------------------------
    }
}