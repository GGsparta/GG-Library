using NaughtyAttributes;
using UnityEngine;

namespace GGL.Components
{
    /// <summary>
    /// Will make a transform follow a target position.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/MISC/Follow Position")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Components) + "." + nameof(FollowPosition) + Settings.COMMON_EXT)]
    public class FollowPosition : MonoBehaviour
    {
        /// <summary>
        /// Defines how the 'follow' behaves
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Linear interpolation. It means the farthest we are from target, the fastest it will move.
            /// </summary>
            LERP,
            /// <summary>
            /// Linear translation. It will always move at the same speed.
            /// </summary>
            LINEAR
        }
        
        /// <value>
        /// Target to follow
        /// </value>
        public Transform target;
        
        /// <value>
        /// See <see cref="Mode"/>.
        /// </value>
        public Mode mode = Mode.LERP;
        
        /// <value>
        /// [LERP MODE ONLY] Flexibility of the bind. 1 will make both positions the same, 0 won't even move...
        /// </value>
        [Range(0f, 1f)] [ShowIf(nameof(mode), Mode.LERP)]
        public float flexibility;
        
        /// <value>
        /// [LINEAR MODE ONLY] Max speed of the follow.
        /// </value>
        [ShowIf(nameof(mode), Mode.LINEAR)]
        public float maxSpeed = 0.2f;
        
        private void Update()
        {
            if(!target) return;
            
            switch (mode)
            {
                case Mode.LERP:
                    transform.position = Vector3.Lerp(target.position, transform.position, flexibility);
                    break;
                case Mode.LINEAR:
                    transform.Translate(Vector3.ClampMagnitude(target.position - transform.position, maxSpeed));
                    break;
            }
        }
    }
}