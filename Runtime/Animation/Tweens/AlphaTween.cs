using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Tweens the alpha of a canvas. For other graphics, check <see cref="ColorTween"/>.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Fade (alpha)")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class AlphaTween : Tween<CanvasGroup>
    {
        #region Consts
        private const float
            ALPHA_VISIBLE = 1f,
            ALPHA_HIDDEN = 0f;
        #endregion

        #region Variables
        #region Editor
        [FormerlySerializedAs("auto")]
        [Header("Tween - Alpha")]
        [SerializeField] [Tooltip("Will show if hidden and hide is visible.")]
        private bool autoFade = true;
        
        [SerializeField]
        [HideIf(nameof(autoFade))] [Range(0,1)] 
        private float finalAlpha;
        
        [SerializeField] [Tooltip("Allows to fade in on enable if 'Trigger Once Enabled' and 'Auto Fade' are activated")]
        private bool hideBeforeEnable = true;
        #endregion

        #region Private
        private float _baseAlpha;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <inheritdoc />
        [ExcludeFromDocFx]
        protected override void OnEnable()
        {
            if (hideBeforeEnable && Target)
                Target.alpha = ALPHA_HIDDEN;
            base.OnEnable();
        }
        #endregion

        #region Public
        /// <inheritdoc />
        public override Tween GenerateTween()
        {
            if (autoFade)
            {
                finalAlpha = Target.alpha < 0.5f ? ALPHA_VISIBLE : ALPHA_HIDDEN;
            }
            return Target.DOFade(finalAlpha, duration);
        }

        /// <inheritdoc />
        public override void SaveState() => _baseAlpha = Target.alpha;

        /// <inheritdoc />
        public override void ResetState() => Target.alpha = _baseAlpha;
        #endregion
        #endregion
    }
}