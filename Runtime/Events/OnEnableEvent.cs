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
        [SerializeField] private float delayBeforeInvoke = 0f;
        
        /// <inheritdoc cref="MonoBehaviour"/>
        private void OnEnable()
        {
            if(delayBeforeInvoke > 0f)
                Invoke(nameof(TryInvoke), delayBeforeInvoke);    
            else
                TryInvoke();
        }
        
        private void OnDisable()
        {
            CancelInvoke();
        }
    }
}