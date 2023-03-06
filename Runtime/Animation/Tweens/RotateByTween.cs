using DG.Tweening;
using UnityEngine;

namespace GGL.Animation.Tweens
{
    /// <summary>
    /// Tweens the rotation of an object around an axis and by the specified angle.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Tweens/Rotate By")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class RotateByTween : Tween<Transform>
    {
        #region Variables
        #region Editor
        [Header("Tween - Rotate By")]
        [SerializeField] [Tooltip("Local axis of rotation")]
        private Vector3 axis = Vector3.up;

        [SerializeField] [Tooltip("Angle of rotation (clockwise)")]
        private float angle = 360f;

        [SerializeField] [Tooltip("Degrees of possible offset to the current axis")]
        private float randomRotationEffect;

        [SerializeField] [Tooltip("Seconds of possible offset to the current duration to randomize speed")]
        private float randomSpeedEffect;
        #endregion

        #region Private
        private float _durationNoise;
        private Quaternion _baseRotation;
        private float _lt;
        private float _t;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <inheritdoc cref="MonoBehaviour" />
        private void Awake() => 
            OnUpdate.AddListener(() => Target.Rotate(axis.normalized, _lt - _t, Space.Self));

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
            _lt = 0f;
            _t = 0f;
            
            return DOTween.To(
                () => _t,
                value =>
                {
                    _lt = _t;
                    _t = value;
                },
                angle,
                duration + _durationNoise);
        }

        /// <inheritdoc />
        public override void SaveState() => _baseRotation = Target.localRotation;

        /// <inheritdoc />
        public override void ResetState() => Target.localRotation = _baseRotation;
        #endregion

        #region Private
        private void Randomize()
        {
            System.Random
                rand = new(Mathf.FloorToInt(transform.position.ToString()
                    .GetHashCode())); // Always same rand for a given position

            // Random rotation
            float
                direction = (float)rand.NextDouble() * Mathf.PI,
                rot = (float)(rand.NextDouble() * randomRotationEffect * 2 - randomRotationEffect);
            Vector3 rotationAxis = new(Mathf.Cos(direction), 0, Mathf.Sin(direction));
            Target.Rotate(rotationAxis, rot);

            // Random speed
            _durationNoise = (float)(rand.NextDouble() * randomSpeedEffect * 2 - randomSpeedEffect);
        }
        #endregion
        #endregion
    }
}