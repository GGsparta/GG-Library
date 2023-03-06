using UnityEngine;
using UnityEngine.Events;

namespace GGL.Events
{
    /// <summary>
    /// Event triggered once MonoBehaviour.OnTriggerEnter is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Trigger")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnTrigger) + Settings.COMMON_EXT)]
    [RequireComponent(typeof(Collider2D))]
    public class OnTrigger : MonoBehaviour
    {
        public UnityEvent onTrigger = new();

        /// <inheritdoc cref="MonoBehaviour"/>
        private void OnTriggerEnter(Collider col) => onTrigger.Invoke();
    }
}