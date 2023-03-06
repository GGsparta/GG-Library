using DG.Tweening;
using UnityEngine;

namespace GGL.Audio.Player
{
    /// <summary>
    /// A Singleton audio player that can crossfade songs.
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/Singletons/Audio/Music Player")]
    [HelpURL(Settings.ARTICLE_URL + "audio" + Settings.COMMON_EXT + "#optimisation")]
    public class MusicPlayer : AbstractPlayer<MusicPlayer>
    {
        /// <value>
        /// A Second audio player used for crossfade transition
        /// </value>
        [SerializeField] protected AudioSource player2;


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
                mixerGroup = Configuration.MusicChannel;

            if (!player)
            {
                player = gameObject.AddComponent<AudioSource>();
                player.outputAudioMixerGroup = mixerGroup;
            }

            if (!player2)
            {
                player2 = gameObject.AddComponent<AudioSource>();
                player2.outputAudioMixerGroup = mixerGroup;
            }
        }

        /// <summary>
        /// Play an audio file on the Singleton audio player with a cross-fading.
        /// </summary>
        /// <param name="clip">Audio file to play.</param>
        /// <param name="volume">Volume in range [0;1].</param>
        /// <param name="crossFade">Cross-fading duration in seconds.</param>
        public static void Play(AudioClip clip, float volume, float crossFade)
        {
            if (DOTween.IsTweening(Instance))
            {
                DOTween
                    .To(() => 0f, _ => { }, default, crossFade / 2)
                    .OnComplete(() => Play(clip, volume, crossFade));
                return;
            }

            AudioSource
                currentPlayer = Instance.player.isPlaying ? Instance.player : Instance.player2,
                nextPlayer = Instance.player.isPlaying ? Instance.player2 : Instance.player;

            if (currentPlayer.isPlaying)
                currentPlayer
                    .DOFade(0, crossFade)
                    .SetTarget(Instance)
                    .OnComplete(() => currentPlayer.Stop());

            nextPlayer.clip = clip;
            nextPlayer.Play();
            nextPlayer
                .DOFade(volume, crossFade)
                .SetTarget(Instance);
        }


        /// <summary>
        /// Stop whatever is being played... with a fading.
        /// </summary>
        /// <param name="fading"></param>
        public static void Stop(float fading)
        {
            if (Instance.player.isPlaying)
                Instance.player
                    .DOFade(0, fading)
                    .SetTarget(Instance)
                    .OnComplete(() => Instance.player.Stop());

            if (Instance.player2.isPlaying)
                Instance.player2
                    .DOFade(0, fading)
                    .SetTarget(Instance)
                    .OnComplete(() => Instance.player.Stop());
        }


        /// <summary>
        /// Play an audio file on the Singleton audio player with a 200ms default cross-fading (it sounds nice :3) .
        /// </summary>
        /// <param name="clip">Audio file to play.</param>
        /// <param name="volume">Volume in range [0;1].</param>
        protected override void PlayClip(AudioClip clip, float volume = 1) => Play(clip, volume, 0.2f);


        /// <summary>
        /// Stop whatever is being played with a 200ms default fading.
        /// </summary>
        protected override void StopClip() => Stop(0.2f);
    }
}