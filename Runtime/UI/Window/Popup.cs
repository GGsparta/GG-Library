using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGL.UI.Window
{
    /// <summary>
    /// Window that shows an interactable background image that can close the window.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/UI/Popup")]
    [HelpURL(Settings.API_URL + nameof(GGL) + "." + nameof(UI) + "." + nameof(UI.Window) + "." + nameof(Popup) + Settings.COMMON_EXT)]
    public class Popup : Window
    {
        [Header("Popup")]
        [SerializeField] private Button background;
        [SerializeField] private float backgroundTargetAlpha = 0.8f;

        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void OnEnable() => background.onClick.AddListener(Close);
        
        /// <inheritdoc cref="MonoBehaviour"/>
        [ExcludeFromDocFx]
        protected virtual void OnDisable() => background.onClick.RemoveListener(Close);

        /// <summary>
        /// Show the background after the method <see cref="Window.Open"/> is called.
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen();
            
            // Show a beautiful background
            Color bgColor = Color.black;
            bgColor.a = 0;
            background.targetGraphic.color = bgColor;
            background.gameObject.SetActive(true);
            background.targetGraphic
                .DOFade(backgroundTargetAlpha, transitionDuration)
                .OnComplete(() => background.interactable = true);
        }

        /// <summary>
        /// Hide the background after the method <see cref="Window.Close"/> is called.
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            
            // Hide the beautiful background
            background.interactable = false;
            background.targetGraphic
                .DOFade(0, transitionDuration)
                .SetDelay(Mathf.Min(.2f, transitionDuration))
                .OnComplete(() => background.gameObject.SetActive(false));
        }
    }
}

