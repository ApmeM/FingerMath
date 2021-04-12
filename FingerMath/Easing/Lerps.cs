namespace FingerMath.Easing
{
    using System;

    /// <summary>
    ///     series of static methods to handle all common tween type structs along with un-clamped lerps for them.
    ///     un-clamped lerps are required for bounce, elastic or other tweens that exceed the 0 - 1 range.
    /// </summary>
    public static class Lerps
    {
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        /// <summary>
        ///     remainingFactorPerSecond is the percentage of the distance it covers every second. should be between 0 and 1.
        ///     if it's 0.25 it means it covers 75% of the remaining distance every second independent of the framerate
        /// </summary>
        /// <returns>The towards.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="remainingFactorPerSecond">Remaining factor per second.</param>
        /// <param name="deltaTime">Delta time.</param>
        public static float LerpTowards(float from, float to, float remainingFactorPerSecond, float deltaTime)
        {
            return Lerp(from, to, 1f - (float)Math.Pow(remainingFactorPerSecond, deltaTime));
        }

        public static float Ease(EaseType easeType, float from, float to, float t, float duration)
        {
            return Lerp(from, to, EaseHelper.Ease(easeType, t, duration));
        }
    }
}