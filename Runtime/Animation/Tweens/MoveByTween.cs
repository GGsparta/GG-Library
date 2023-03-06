using DG.Tweening;
using UnityEngine;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Tweens the position of an object by the specified distance.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Move By")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class MoveByTween : Tween<Transform>
    {
        #region Variables
        #region Editor
        [Header("Tween - Move By")]
        [SerializeField]
        private Vector3 movement = Vector3.up;
        #endregion

        #region Private
        private Vector3 _basePosition;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <inheritdoc />
        public override Tween GenerateTween() => Target.DOLocalMove(Target.localPosition + movement, duration);

        /// <inheritdoc />
        public override void SaveState() => _basePosition = Target.localPosition;

        /// <inheritdoc />
        public override void ResetState() => Target.localPosition = _basePosition;
        #endregion
        #endregion
    }
}