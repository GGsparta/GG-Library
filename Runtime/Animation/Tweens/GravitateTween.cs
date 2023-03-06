using DG.Tweening;
using UnityEngine;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Rotates an object around the local Y axis of another.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Gravitate")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class GravitateTween : Tween<Transform>
    {
        #region Variables
        #region Editor
        [Header("Tween - Gravitate")]
        [SerializeField] [Tooltip("If null, uses parent transform")]
        private Transform centerOfGravity;

        [SerializeField]
        private float angle = 360f;

        [SerializeField] [Tooltip("If checked, it will only update position")]
        private bool freezeRotation = true;

        [SerializeField] [Tooltip("Seconds of possible offset to the current duration to randomize speed")]
        private float randomSpeedEffect;
        #endregion

        #region Private
        private float _durationNoise;
        private Quaternion _baseRotation;
        private Vector3 _basePosition;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <inheritdoc />
        [ExcludeFromDocFx]
        protected override void OnEnable()
        {
            Randomize();
            base.OnEnable();
        }
        #endregion

        #region Public
        /// <inheritdoc />
        public override Tween GenerateTween()
        {
            Vector3 upAxis = centerOfGravity ? centerOfGravity.up : Target.parent.up;
            Vector3 point = centerOfGravity ? centerOfGravity.position : Target.parent.position;
            float lt = 0f, t = 0f;
            startTime += _durationNoise;
            return DOTween.To(() => t, value =>
                {
                    lt = t;
                    t = value;
                }, angle, duration + _durationNoise)
                .OnUpdate(() =>
                {
                    Target.RotateAround(point, upAxis, t - lt);
                    if (freezeRotation) Target.localRotation = _baseRotation;
                });
        }

        /// <inheritdoc />
        public override void SaveState()
        {
            _baseRotation = Target.localRotation;
            _basePosition = Target.localPosition;
        }

        /// <inheritdoc />
        public override void ResetState()
        {
            Target.localRotation = _baseRotation;
            Target.localPosition = _basePosition;
        }
        #endregion

        #region Private
        private void Randomize()
        {
            System.Random
                rand = new(Mathf.FloorToInt(transform.position.ToString()
                    .GetHashCode())); // Always same rand for a given position

            // Random speed
            _durationNoise = (float)(rand.NextDouble() * randomSpeedEffect * 2 - randomSpeedEffect);
        }
        #endregion
        #endregion
    }
}