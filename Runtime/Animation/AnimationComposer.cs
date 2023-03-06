using System.Collections.Generic;
using System.Linq;
using GGL.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GGL.Animation
{
    /// <summary>
    /// <see cref="TriggerBehaviour"/> that can trigger/kill a set of other <see cref="TriggerBehaviour"/>.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Animation/Animation Composer")]
    [HelpURL(Settings.ARTICLE_URL + "tweens" + Settings.COMMON_EXT)]
    public class AnimationComposer : TriggerBehaviour
    {
        #region Variables
        #region Editor
#if UNITY_EDITOR
        [SerializeField] [TextArea] [Tooltip("[reading purposes]")]
        // ReSharper disable once NotAccessedField.Local
#pragma warning disable 0414
        private string description = "Description...";
#pragma warning restore 0414
#endif

        /// <value>
        /// Triggers to trigger (tricky, ain't it? 😛)
        /// </value>
        [SerializeField] [Tooltip("Triggers to trigger once triggered")]
        protected TriggerBehaviour[] triggers;

        [Foldout("Events")]
        [SerializeField] [Tooltip("This event is triggered when the script is triggered")]
        private UnityEvent onTrigger;
        #endregion

        #region Public
        /// <value>
        /// This event is triggered when the script is triggered
        /// </value>
        private UnityEvent OnTrigger => onTrigger;
        #endregion

        #region Private
        private Dictionary<TriggerBehaviour, bool> _triggersState = new();
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected override void OnEnable()
        {
            foreach (KeyValuePair<TriggerBehaviour, bool> element in _triggersState)
                element.Key.enabled = element.Value;

            base.OnEnable();
        }

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnDisable()
        {
            _triggersState.Clear();

            foreach (TriggerBehaviour trigger in triggers)
            {
                _triggersState[trigger] = trigger.enabled;
                trigger.enabled = false;
            }
        }

        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnValidate()
        {
            if (triggers != null && triggers.All(t => t != null)) return;

            if (isActiveAndEnabled) triggers = GetComponentsInChildren<TriggerBehaviour>().Where(t => t != this).ToArray();
            else _triggersState.Clear();
        }
        #endregion

        #region Public
        /// <summary>
        /// Trigger every triggers.
        /// </summary>
        [ContextMenu(nameof(Trigger))]
        public override void Trigger()
        {
            if (!isActiveAndEnabled)
                return;

            onTrigger.Invoke();
            triggers.ForEach(t => t.Trigger());
        }

        /// <summary>
        /// Kill all the triggers.
        /// </summary>
        [ContextMenu(nameof(Kill))]
        public override void Kill() => triggers.ForEach(t => t.Kill());
        #endregion
        #endregion
    }
}