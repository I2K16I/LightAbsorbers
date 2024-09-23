using UnityEngine.InputSystem;

namespace BSA
{
    public enum Rumble
    {
        None = 0,
        Light = 1,
        Medium = 2,
        Strong = 3,
    }

    public static class RumbleExtensions
    {
        public static void ResetRumble(this Gamepad pad)
            => SetRumble(pad, Rumble.None);

        public static void SetRumble(this Gamepad pad, Rumble rumbleType)
            => RumbleManager.Instance.SetRumble(pad, rumbleType);

        public static void SetRumbleForDuration(this Gamepad pad, Rumble rumbleType, float duration)
            => RumbleManager.Instance.SetRumbleForDuration(pad, rumbleType, duration);
    }
}