#if UNITY_EDITOR
#endif
using NaughtyAttributes;
using UnityEngine;

namespace GGL.Components
{
    /// <summary>
    /// Avoids a root <see cref="GameObject"/> to be destroyed while loading a scene. 
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/MISC/Don't Destroy On Load")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Components) + "." + nameof(Components.DontDestroyOnLoad) + Settings.COMMON_EXT)]
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            if (transform.parent) 
                Debug.LogError($"{name} won't be destroyed on load but it should be a root scene GameObject!");
            DontDestroyOnLoad(gameObject);
        }

#if UNITY_EDITOR
        private bool ShowInfoBox => transform.parent;
        [InfoBox("This component should be on a root scene GameObject!", EInfoBoxType.Error)]
        [ShowIf(nameof(ShowInfoBox))] [InspectorName("Error")] [SerializeField] [BoxGroup("Be careful!")] [ReadOnly]
        // ReSharper disable once NotAccessedField.Local
#pragma warning disable 0414
        private string error;
#pragma warning restore 0414
#endif
    }
}