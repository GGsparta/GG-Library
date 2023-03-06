using UnityEngine;

namespace GGL.Components
{
    /// <summary>
    /// Will make a transform look at a target.
    /// </summary>
    /// <remarks>If no target set, will look at camera.</remarks>
    [AddComponentMenu(Settings.NAME + "/MISC/Look At")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Components) + "." + nameof(LookAt) + Settings.COMMON_EXT)]
    public class LookAt : MonoBehaviour
    {
        /// <value>
        /// Target to look at.
        /// </value>
        public Transform target;
        
        /// <value>
        /// Offset applied to the final rotation.
        /// </value>
        public Vector3 rotationOffset;

        private void OnEnable()
        {
            if (!target)
                target = Camera.current.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(target);
            transform.Rotate(rotationOffset);
        }
    }
}