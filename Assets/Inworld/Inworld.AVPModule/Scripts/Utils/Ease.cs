using System;
using UnityEngine;

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{
    /// <summary>
    ///     Static class containing general easing functions
    /// </summary>
    public static class Ease
    {
        public enum EaseType
        {
            In,
            Out,
            InOut
        }

        /// <summary>
        ///     Apply an ease to a interpolation value from zero to one
        /// </summary>
        /// <param name="easeType">The easing type to use - In, Out or InOut</param>
        /// <param name="power">The easing power to use (2 for quadratic, 3 for cubic, etc)</param>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        /// <returns>An eased version of the interpolation parameter t</returns>
        public static float Transform(EaseType easeType, int power, float t)
        {
            switch (easeType) {
                case EaseType.In:
                    return In(power, t);
                case EaseType.Out:
                    return Out(power, t);
                case EaseType.InOut:
                    return InOut(power, t);
                default:
                    throw new FormatException("Unexpected value provided for EaseType: " + easeType);
            }
        }

        /// <summary>
        ///     Apply an ease in to an interpolation value from zero to one
        /// </summary>
        /// <param name="power">The easing power to use (2 for quadratic, 3 for cubic, etc)</param>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float In(int power, float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            return power < 2 ? t : Mathf.Pow(t, power);
        }

        /// <summary>
        ///     Apply an ease out to an interpolation value from zero to one
        /// </summary>
        /// <param name="power">The easing power to use (2 for quadratic, 3 for cubic, etc)</param>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float Out(int power, float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            if (power < 2) return t;

            return (power % 2 == 0 ? -1f : 1f) * Mathf.Pow(t - 1f, power) + 1f;
        }

        /// <summary>
        ///     Apply an ease in and out to an interpolation value from zero to one
        /// </summary>
        /// <param name="power">The easing power to use (2 for quadratic, 3 for cubic, etc)</param>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float InOut(int power, float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            if (power < 2) return t;

            if (t < 0.5f) return Mathf.Pow(2f, power - 1) * Mathf.Pow(t, power);
            return (power % 2 == 0 ? -1f : 1f) * Mathf.Pow(2f, power - 1) * Mathf.Pow(t - 1f, power) + 1f;
        }

        /// <summary>
        ///     Apply a quadratic ease in
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuadraticIn(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            return t * t;
        }

        /// <summary>
        ///     Apply a quadratic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuadraticOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            t--;
            return -1f * t * t + 1f;
        }

        /// <summary>
        ///     Apply a quadratic ease in + quadratic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuadraticInOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            if (t < 0.5f) return 2f * t * t;
            t--;
            return -2f * t * t + 1f;
        }

        /// <summary>
        ///     Apply a quadratic ease with a factor that blends between in and out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        /// <param name="inout">
        ///     The blend from 0 (completely in) to 1 (completely out). This represents the point at which the function returns
        ///     0.5.
        ///     Note that it's clamped from 0.293 to 0.707 [1 - sqrt(2) / 2] to [sqrt(2) / 2]
        /// </param>
        public static float QuadraticAdaptive(float t, float inout)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            inout = Mathf.Lerp(0.707f, 0.293f, inout);
            var x = ((0.5f / inout) - 1f) / (inout - 1f);
            return x * t * t + (1 - x) * t;
        }

        /// <summary>
        ///     Apply an adaptive ease in/out function with expressive parameters for the incoming and outgoing 'velocity'.
        ///     By @FarbsMcFarbs at https://twitter.com/FarbsMcFarbs/status/1456830625617432576
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        /// <param name="inVelocity">
        ///     The subjective 'speed' at which the function moves away from zero. At high values, can cause
        ///     the function to overshoot one
        /// </param>
        /// <param name="outVelocity">
        ///     The subjective 'speed' at which the function approaches one. At high values, can cause the
        ///     function to go below zero at the beginning of its region
        /// </param>
        public static float CubicBiAdaptive(float t, float inVelocity, float outVelocity)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            var pow3 = Mathf.Pow(outVelocity + t - 2f, 3f);
            var pow2 = Mathf.Pow(-2f * inVelocity - outVelocity + 3f, 2f);
            var pow1 = inVelocity * t;
            return pow3 + pow3 + pow1;
        }

        /// <summary>
        ///     Apply a cubic ease in
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float CubicIn(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            return t * t * t;
        }

        /// <summary>
        ///     Apply a cubic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float CubicOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            t--;
            return t * t * t + 1f;
        }

        /// <summary>
        ///     Apply a cubic ease in + cubic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float CubicInOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            if (t < 0.5f) return 4f * t * t * t;
            t--;
            return 4f * t * t * t + 1f;
        }

        /// <summary>
        ///     Apply a quartic ease in
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuarticIn(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            return t * t * t * t;
        }

        /// <summary>
        ///     Apply a quartic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuarticOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            t--;
            return -1f * t * t * t * t + 1f;
        }

        /// <summary>
        ///     Apply a quartic ease in + quartic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuarticInOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            if (t < 0.5f) return 8f * t * t * t * t;
            t--;
            return -8f * t * t * t * t + 1f;
        }

        /// <summary>
        ///     Apply a quintic ease in
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuinticIn(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            return t * t * t * t * t;
        }

        /// <summary>
        ///     Apply a quintic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuinticOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            t--;
            return t * t * t * t * t + 1f;
        }

        /// <summary>
        ///     Apply a quintic ease in + quintic ease out
        /// </summary>
        /// <param name="t">The interpolation parameter to apply easing to</param>
        public static float QuinticInOut(float t)
        {
            if (t < 0 || t > 1) t = Mathf.Clamp01(t);
            if (t < 0.5f) return 16f * t * t * t * t * t;
            t--;
            return 16f * t * t * t * t * t + 1f;
        }

        /// <summary>
        ///     Turn a parameter from zero to one into a time-independent interpolation value.
        ///     Intended for situations like transform.position = Vector3.Lerp(transform.position, targetPosition,
        ///     GetEaseFactor(LerpRate));
        ///     A replacement for transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * LerpRate),
        ///     where LerpRate is a value between around zero and 12 (and has nonlinear relationship to time-to-target)
        /// </summary>
        /// <param name="t">Lerp factor. 0 = no lerp, 1 = instant lerp. 0.25 = slow lerp, 0.5 = average lerp, 0.75 = fast lerp</param>
        public static float GetEaseFactor(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            return Time.deltaTime * 3f * Mathf.Pow(2f, 5f * t - 3f);
        }
    }
}