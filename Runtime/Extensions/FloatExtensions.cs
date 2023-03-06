using UnityEngine;

namespace GGL.Extensions
{ 
    public static class FloatExtensions {
        /// <summary>
        /// Remaps a value from a range [a,b] to a range [c,d].
        /// </summary>
        public static float Remap(this float value, float a, float b, float c, float d) 
            => (value - a) / (b - a) * (d - c) + c;

        /// <summary>
        /// Checks if value is within a range [a,b].
        /// </summary>
        public static bool IsWithin(this float value, float a, float b)
            => a <= value && value <= b;

        #region MATHF Overloads
        /// <inheritdoc cref="Mathf.Clamp(float,float,float)"/>
        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
        /// <summary><see cref="Clamp(float,float,float)"/> but min=0</summary>
        public static float Clamp(this float value, float max) => Mathf.Clamp(value, 0f, max);
        /// <inheritdoc cref="Mathf.Abs(float)"/>
        public static float Abs(this float value) => Mathf.Abs(value);
        /// <inheritdoc cref="Mathf.RoundToInt(float)"/>
        public static int Round(this float value) => Mathf.RoundToInt(value);
        #endregion
    }
}