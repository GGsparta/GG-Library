using UnityEngine;

namespace GGL.Audio.Player
{
    /// <summary>
    /// A basic Singleton audio player to play sounds.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Singletons/Audio/Sound Player")]
    [HelpURL(Settings.ARTICLE_URL + "audio" + Settings.COMMON_EXT + "#optimisation")]
    public class SoundPlayer : AbstractPlayer<SoundPlayer>
    {
        /// <inheritdoc cref="MonoBehaviour" />
        private void OnValidate() => InitDefault();
        /// <inheritdoc />
        [ExcludeFromDocFx]
        protected override void Awake()
        {
            base.Awake();
            InitDefault();
        }

        /// <summary>
        /// Initialization for default configuration (e.g. create by Singleton access).
        /// Be aware that it should not erase any previous configuration.
        /// </summary>
        private void InitDefault()
        {
            if (!mixerGroup) 
                mixerGroup = Configuration.SoundsChannel;

            if (!player)
            {
                player = gameObject.AddComponent<AudioSource>();
                player.outputAudioMixerGroup = mixerGroup;
            }
        }

        /// <summary>
        /// Play an audio file on the Singleton audio player.
        /// </summary>
        /// <param name="clip">Audio file to play.</param>
        /// <param name="volume">Volume in range [0;1].</param>
        protected override void PlayClip(AudioClip clip, float volume = 1)
        {
            player.Stop();
            player.volume = volume;
            player.clip = clip;
            player.Play();
        }

        /// <summary>
        /// Stop whatever is being played.
        /// </summary>
        protected override void StopClip() => player.Stop();
    }
}