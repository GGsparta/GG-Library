using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Tweens the color on any Graphic component.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Fade (color)")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class ColorTween : Tween<Graphic>
    {
        #region Variables
        #region Editor
        [Header("Tween - Color")]
        [SerializeField] [Tooltip("Ignored if clear (0,0,0,0)")]
        private Color fromColor = Color.clear;

        [SerializeField]
        private Color toColor = Color.white;
        #endregion

        #region Private
        private Color _startColor = Color.clear;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <inheritdoc />
        public override Tween GenerateTween()
        {
            if (!fromColor.Equals(Color.clear))
                Target.color = fromColor;
            return Target.DOColor(toColor, duration);
        }

        /// <inheritdoc />
        public override void SaveState() => _startColor = Target.color;

        /// <inheritdoc />
        public override void ResetState() => Target.color = _startColor;
        #endregion
        #endregion
    }
}