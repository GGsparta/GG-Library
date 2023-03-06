using UnityEngine;

namespace GGL.Components
{
    /// <summary>
    /// Provide a method <see cref="Kill"/> that will kill the app.
    /// </summary>
    /// <remarks>Does not work on WebGL.</remarks>
    [AddComponentMenu(Settings.NAME + "/MISC/App Killer")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(Components) + "." + nameof(AppKiller) + Settings.COMMON_EXT)]
    public class AppKiller : MonoBehaviour
    {
        /// <summary>
        /// Kill the app.
        /// </summary>
        /// <remarks>Does not work on WebGL.</remarks>
        public void Kill()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}