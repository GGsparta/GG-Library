using DG.Tweening;
using UnityEngine;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Tweens the rotation of an object to a given Euler rotation.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Rotate To")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class RotateToTween : Tween<Transform>
    {
        #region Variables
        #region Editor
        [Header("Tween - Rotate To")]
        [SerializeField] private Vector3 rotation = Vector3.forward;
        #endregion

        #region Private
        private Quaternion _baseRotation;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <inheritdoc />
        public override Tween GenerateTween() =>
            Target.DOLocalRotateQuaternion(Target.localRotation * Quaternion.Euler(rotation), duration);

        /// <inheritdoc />
        public override void SaveState() => _baseRotation = Target.localRotation;

        /// <inheritdoc />
        public override void ResetState() => Target.localRotation = _baseRotation;
        #endregion
        #endregion
    }
}