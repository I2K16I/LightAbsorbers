using System;
using System.Collections;
using UnityEngine;

namespace BSA
{
    public enum Axis
    {
        X,
        Y,
        Z,
        W
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
        public static void SetPosition(this Transform t, Axis axis, float value)
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

            t.position = axis switch
            {
                Axis.X => new Vector3(value, pos.y, pos.z),
                Axis.Y => new Vector3(pos.x, value, pos.z),
                Axis.Z => new Vector3(pos.x, pos.y, value),
                _ => throw new NotImplementedException(),
            };
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

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}