using UnityEngine;

namespace GGL.Events
{
    /// <summary>
    /// Platform-specific event triggered once MonoBehaviour.OnDestroy is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Destroy Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnDestroyEvent) + Settings.COMMON_EXT)]
    public class OnDestroyEvent : PlatformEvent
    {
        /// <inheritdoc cref="MonoBehaviour"/>
        private void OnDestroy() => TryInvoke();
    }
}