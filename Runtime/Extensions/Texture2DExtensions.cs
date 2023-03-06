using System;
using UnityEngine;

namespace GGL.Extensions
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Paint and apply color to a texture.
        /// </summary>
        /// <remarks>Old pixels are interpolated with the color alpha.</remarks>
        public static Texture2D Paint(this Texture2D texture, Color color, Func<int, int, bool> condition = null)
        {
            Color[] pix = texture.GetPixels();
            int w = texture.width, h = texture.height;

            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    int pos = r * w + c;
                    if (condition != null && !condition.Invoke(r, c)) continue;
                    pix[pos] = Color.Lerp(pix[pos], color.ToRGB(), color.a);
                }
            }

            texture.SetPixels(pix);
            texture.Apply();

            return texture;
        }
        
        /// <summary>
        /// Paint and apply a colored border to a texture.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="boderColor"></param>
        /// <param name="borderWidth"></param>
        /// <returns></returns>
        public static Texture2D PaintBorder(this Texture2D texture, Color boderColor, int borderWidth) =>
            Paint(texture, boderColor, (row, col) => 
                row < borderWidth || 
                row >= texture.height - borderWidth ||
                col < borderWidth || 
                col >= texture.width - borderWidth);
    }
}