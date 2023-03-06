using UnityEngine;

namespace GGL.Events
{
    /// <summary>
    /// Platform-specific event triggered once MonoBehaviour.Start is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Start Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnStartEvent) + Settings.COMMON_EXT)]
    public class OnStartEvent : PlatformEvent
    {
        /// <inheritdoc cref="MonoBehaviour"/>
        private void Start() => TryInvoke();
    }
}