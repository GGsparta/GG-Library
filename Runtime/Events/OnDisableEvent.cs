using UnityEngine;

namespace GGL.Events
{
    /// <summary>
    /// Platform-specific event triggered once MonoBehaviour.OnDisable is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Disable Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnDisableEvent) + Settings.COMMON_EXT)]
    public class OnDisableEvent : PlatformEvent
    {
        /// <inheritdoc cref="MonoBehaviour"/>
        private void OnDisable() => TryInvoke();
    }
}