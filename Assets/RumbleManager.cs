using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BSA
{
    public class RumbleManager : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        private static RumbleManager _instance;

        // --- Properties ---------------------------------------------------------------------------------------------
        public static RumbleManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    return new GameObject("RumbleManager").AddComponent<RumbleManager>();
                }

                return _instance;
            }
        }

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void SetRumble(Gamepad pad, Rumble rumbleType)
        {
            if(pad == null)
                return;

            switch(rumbleType)
            {
                case Rumble.None:
                    pad.ResetHaptics();
                    break;
                case Rumble.Light:
                    pad.SetMotorSpeeds(0.0f, 0.55f);
                    break;
                case Rumble.Medium:
                    pad.SetMotorSpeeds(0.2f, 0.75f);
                    break;
                case Rumble.Strong:
                    pad.SetMotorSpeeds(0.75f, 0.75f);
                    break;
            }
        }

        public void SetRumbleForDuration(Gamepad pad, Rumble rumbleType, float duration)
        {
            if(pad == null)
                return;

            SetRumble(pad, rumbleType);
            StartCoroutine(ResetRoutine());

            IEnumerator ResetRoutine()
            {
                yield return new WaitForSeconds(duration);
                pad.ResetHaptics();
            }
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}