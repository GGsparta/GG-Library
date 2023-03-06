using UnityEngine;

namespace GGL.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Removes alpha from color
        /// </summary>
        public static Color ToRGB(this Color color) => new(color.r, color.g, color.b);
    }
}