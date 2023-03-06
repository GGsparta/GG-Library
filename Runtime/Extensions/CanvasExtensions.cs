using UnityEngine;

namespace GGL.Extensions
{
    public static class CanvasExtensions
    {
        /// <summary>
        /// Returns main camera for canvas.
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns>Camera of the canvas or null if bad canvas usage (e.g. <see cref="RenderMode.ScreenSpaceOverlay"/>).</returns>
        /// <remarks>This method may call <see cref="Camera.main"/></remarks>
        public static Camera GetCanvasCamera(this Canvas canvas)
        {
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay) 
                return null;
            
            return canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : Camera.main;
        }
    }
}