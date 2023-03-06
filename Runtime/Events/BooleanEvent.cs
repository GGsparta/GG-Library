using UnityEngine;
using UnityEngine.Events;

namespace GGL.Events
{
    /// <summary>
    /// Redirects a boolean Unity event into the event corresponding to its result.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/Boolean Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(BooleanEvent) + Settings.COMMON_EXT)]
    public class BooleanEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onTrue;
        [SerializeField] private UnityEvent onFalse;

        /// <summary>
        /// Invoke the event from another one.
        /// </summary>
        /// <param name="boolean">Result of your other event.</param>
        public void Invoke(bool boolean) => (boolean ? onTrue : onFalse).Invoke();
    }
}