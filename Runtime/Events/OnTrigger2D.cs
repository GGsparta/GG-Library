using UnityEngine;
using UnityEngine.Events;

namespace GGL.Events
{
    /// <summary>
    /// Event triggered once MonoBehaviour.OnTriggerEnter2D is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Trigger (2D)")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnTrigger2D) + Settings.COMMON_EXT)]
    [RequireComponent(typeof(Collider2D))]
    public class OnTrigger2D : MonoBehaviour
    {
        public UnityEvent onTrigger = new();

        /// <inheritdoc cref="MonoBehaviour"/>
        private void OnTriggerEnter2D(Collider2D col) => onTrigger.Invoke();
    }
}