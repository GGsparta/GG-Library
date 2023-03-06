using DG.Tweening;
using UnityEngine;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Tweens the scale of an object.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Scale")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class ScaleTween : Tween<Transform>
    {
        #region Variables
        #region Editor
        [Header("Tween - Scale")]
        [SerializeField]
        private Vector3 scale = Vector3.one;
        #endregion

        #region Private
        private Vector3 _baseScale;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <inheritdoc />
        public override Tween GenerateTween() => Target.DOScale(scale, duration);

        /// <inheritdoc />
        public override void SaveState() => _baseScale = Target.localScale;

        /// <inheritdoc />
        public override void ResetState() => Target.localScale = _baseScale;
        #endregion
        #endregion
    }
}