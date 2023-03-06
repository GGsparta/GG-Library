using UnityEngine;

namespace GGL.Utility
{
    public static class Texture2DUtility
    {
        /// <summary>
        /// Creates a texture filled with the given color.
        /// </summary>
        public static Texture2D CreateTexture2D(int width, int height, Color color)
        {
            Texture2D tex = new(width, height);
            Color[] p = tex.GetPixels();
            for (int i = 0; i < p.Length; i++)
                p[i] = color;
            tex.SetPixels(p);
            tex.Apply();
            return tex;
        }
    }
}