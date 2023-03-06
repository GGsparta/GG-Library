using GGL.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace GGL.Audio.Player
{
    /// <summary>
    /// Generic audio player you can implement to create you own Singleton audio player.
    /// </summary>
    /// <typeparam name="T">Your script so that it can be set as Singleton.</typeparam>
    /// <example><see cref="MusicPlayer"/> and <see cref="SoundPlayer"/> are good examples of what you can do.</example>
    /// <remarks>To learn how to properly inherit this class, check the HelpURL attribute.</remarks>
    [HelpURL(Settings.ARTICLE_URL + "audio" + Settings.COMMON_EXT + "#optimisation")]
    public abstract class AbstractPlayer<T> : SingletonBehaviour<T> where T : AbstractPlayer<T>
    {
        /// <value>
        /// The channel used to emit the audio.
        /// </value>
        [SerializeField] protected AudioMixerGroup mixerGroup;
        
        /// <value>
        /// The associated player: it should be script's object or its children to benefit the cross-scene advantage.
        /// </value>
        [SerializeField] protected AudioSource player;

        /// <inheritdoc cref="Play"/>
        protected abstract void PlayClip(AudioClip clip, float volume = 1f);
        
        /// <inheritdoc cref="Stop"/>
        protected abstract void StopClip();

        /// <summary>
        /// Play an audio file.
        /// </summary>
        /// <param name="clip">Audio file to play.</param>
        /// <param name="volume">Volume in range [0;1].</param>
        public static void Play(AudioClip clip, float volume = 1f) => Instance.PlayClip(clip, volume);
        
        /// <summary>
        /// Stop whatever is being played.
        /// </summary>
        public static void Stop() => Instance.StopClip();
    }
}