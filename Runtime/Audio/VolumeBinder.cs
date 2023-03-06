using GGL.Extensions;
using UnityEngine;

namespace GGL.Audio
{
    /// <summary>
    /// Generic way to edit and bind the volume of a channel (AudioMixerGroup) to the local storage
    /// </summary>
    /// <example><see cref="VolumeSlider"/> is a good example of what you can do.</example>
    [HelpURL(Settings.ARTICLE_URL + "audio" + Settings.COMMON_EXT + "#interaction")]
    public class VolumeBinder : MonoBehaviour
    {
        /// <value>
        /// Minimal value used for setting volume.
        /// </value>
        protected const float MIN_VOLUME = 0.0001f;
        /// <value>
        /// Maximal value used for setting volume.
        /// </value>
        protected const float MAX_VOLUME = 1f;

        /// <value>
        /// The channel to bind and edit.
        /// </value>
        [SerializeField] protected AudioChannel channel;

        /// <value>
        /// The current volume in range [0;1].
        /// </value>
        protected float Value { get; private set; }


        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void Awake() => Init();
        
        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void OnValidate() => channel?.Validate();

        /// <summary>
        /// Read the local storage to initialize the channel volume.
        /// This method is called once the component is enabled.
        /// </summary>
        /// <remarks>If not enabled, do not forget to call this method once the scene is loaded.</remarks>
        public virtual void Init()
        {
            Value = channel?.Init() ?? 1f;
        }


        /// <summary>
        /// Set a new volume to channel and local storage
        /// </summary>
        /// <param name="value">Volume in range ]0;1]</param>
        public virtual void Set(float value)
        {
            if (channel == null || !Application.isPlaying)
                return;
            
            channel.Set(Value = value.Clamp(MIN_VOLUME, MAX_VOLUME));
        }
    }
}