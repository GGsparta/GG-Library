using UnityEngine;

namespace GGL.Extensions
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Returns the camera rect in world position.
        /// </summary>
        /// <param name="camera"></param>
        public static Rect GetCameraRectWorldSpace(this Camera camera)
        {
            Vector2
                min = camera.ViewportToWorldPoint(Vector3.zero),
                max = camera.ViewportToWorldPoint(Vector3.right + Vector3.up);

            return new Rect(min, max - min);
        }
    }
}