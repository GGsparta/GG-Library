using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GGL.Events
{
    /// <summary>
    /// A Unity event that will be triggered depending on the platform defined.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/Platform Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(PlatformEvent) + Settings.COMMON_EXT)]
    public abstract class PlatformEvent : MonoBehaviour
    {
        [Tooltip("If none set, event is fired anyway")]
        [SerializeField] private RuntimePlatform[] platforms = Array.Empty<RuntimePlatform>();
        [SerializeField] private UnityEvent onInvoke = new();

        public UnityEvent OnInvoke => onInvoke;

        /// <summary>
        /// Invoke the event if the current platform matches ith one defined by user. 
        /// </summary>
        public void TryInvoke()
        {
            RuntimePlatform platform = Application.platform;
            if (platforms.Length == 0 || platforms.Any(p => platform.Equals(p)))
                onInvoke.Invoke();
        }
    }
}