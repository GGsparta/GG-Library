using UnityEngine;

namespace GGL.Events
{
    /// <summary>
    /// Platform-specific event triggered once MonoBehaviour.Awake is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Awake Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnAwakeEvent) + Settings.COMMON_EXT)]
    public class OnAwakeEvent : PlatformEvent
    {
        /// <inheritdoc cref="MonoBehaviour"/>
        private void Awake() => TryInvoke();
    }
}