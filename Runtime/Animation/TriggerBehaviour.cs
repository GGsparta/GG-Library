using UnityEngine;

namespace GGL.Animation
{
    /// <summary>
    /// Behaviour that can trigger and kill a piece of code.
    /// </summary>
    /// <remarks>Base class for every <see cref="Tween{T}"/>, so that animations can be triggered. However you can use this class to trigger you own stuff.</remarks>
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public abstract class TriggerBehaviour : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <value >Once the component is ative in scene, it will be triggered.</value>
        [SerializeField]
        protected bool triggerOnceEnabled;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnEnable()
        {
            if(triggerOnceEnabled) Trigger();
        }
        #endregion

        #region Public
        /// <summary>
        /// Whatever you want to trigger.
        /// </summary>
        public abstract void Trigger();

        /// <summary>
        /// Some way to kill what you triggered.
        /// </summary>
        public abstract void Kill();
        #endregion
        #endregion

    }
}