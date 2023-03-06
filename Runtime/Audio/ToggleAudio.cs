using GGL.Audio.Player;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace GGL.Audio
{
    /// <summary>
    /// Behaviour that associates a sound to a toggle. Just by adding this component, everything is set up.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleAudio : MonoBehaviour
    {
        /// <summary>
        /// Define how the script will behaviour to play an audio file.
        /// </summary>
        protected enum Mode
        {
            /// <summary>
            /// In this mode, the button will play an audio file using the <see cref="SoundPlayer"/> Singleton.
            /// </summary>
            DEFAULT,

            /// <summary>
            /// In this mode, the button will play an associated AudioSource.
            /// </summary>
            CUSTOM
        }

        /// <value>
        /// Define how the script will behaviour to play an audio file.
        /// </value>
        [SerializeField] protected Mode mode = Mode.DEFAULT;

        /// <value>
        /// In DEFAULT mode, volume to play the audio file.
        /// </value>
        [ShowIf(nameof(mode), Mode.DEFAULT)] [SerializeField]
        protected float volume = 1f;

        /// <value>
        /// In DEFAULT mode, audio file to play.
        /// </value>
        [ShowIf(nameof(mode), Mode.DEFAULT)] [SerializeField]
        protected AudioClip clip;

        /// <value>
        /// In CUSTOM mode, AudioSource to play.
        /// </value>
        [ShowIf(nameof(mode), Mode.CUSTOM)] [SerializeField]
        protected AudioSource source;


        /// <inheritdoc cref="MonoBehaviour" />
        [ExcludeFromDocFx]
        protected virtual void Awake() => GetComponent<Toggle>().onValueChanged.AddListener(_ => PlaySound());

        /// <summary>
        /// Play a sound depending on the mode. Defaultly called by the associated button once clicked. 
        /// </summary>
        public virtual void PlaySound()
        {
            switch (mode)
            {
                case Mode.DEFAULT:
                    SoundPlayer.Play(clip, volume);
                    break;
                case Mode.CUSTOM:
                    source.Play();
                    break;
            }
        }
    }
}