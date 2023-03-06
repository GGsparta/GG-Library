using UnityEngine;

namespace GGL.Events
{
    /// <summary>
    /// Platform-specific event triggered once MonoBehaviour.OnEnable is called.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Events/On Enable Event")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Events) + "." + nameof(OnEnableEvent) + Settings.COMMON_EXT)]
    public class OnEnableEvent : PlatformEvent
    {
        /// <inheritdoc cref="MonoBehaviour"/>
        private void OnEnable() => TryInvoke();
    }
}