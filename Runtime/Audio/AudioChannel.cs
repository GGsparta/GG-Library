using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace GGL.Audio
{
    /// <summary>
    /// Displayable class that bind a mixer group to its stored volume.
    /// </summary>
    [Serializable]
    public class AudioChannel
    {
        // ReSharper disable once NotAccessedField.Local
        [HideInInspector] [SerializeField] private string name;
        
        [Tooltip("Output audio mixer")] 
        [SerializeField] private AudioMixerGroup channel;
        
        [Tooltip("Name of the volume variable in mixer")] 
        [SerializeField] private string volumeProperty;
        
        private string PlayerPrefsKey => $"{volumeProperty}_Volume";

        /// <summary>
        /// Create a channel from its output mixer.
        /// </summary>
        /// <param name="channel">Output audio mixer.</param>
        /// <param name="volumeProperty">Specify if different from channel name.</param>
        public AudioChannel(AudioMixerGroup channel, string volumeProperty = "")
        {
            this.channel = channel;
            this.volumeProperty = volumeProperty;
            Validate();
        }
        
        [ExcludeFromDocFx]
        public void Validate()
        {
            if (string.IsNullOrEmpty(volumeProperty) && channel)
                volumeProperty = channel.name;
            name = channel.name;
        }
        
        /// <summary>
        /// Read the local storage to initialize the channel volume.
        /// This method is called once the component is enabled.
        /// </summary>
        /// <remarks>If not enabled, do not forget to call this method once the scene is loaded.</remarks>
        public float Init(float defaultVolume = 1f)
        {
            if (channel)
            {
                Validate();
                
                float volume = PlayerPrefs.GetFloat(PlayerPrefsKey, default);
                channel.audioMixer.SetFloat(volumeProperty, volume);
                defaultVolume = Mathf.Pow(10, volume / 20);
            }

            return defaultVolume;
        }
        
        /// <summary>
        /// Set a new volume to channel and local storage
        /// </summary>
        /// <param name="value">Volume in range ]0;1]</param>
        public virtual void Set(float value)
        {
            if (!channel || !Application.isPlaying)
                return;
            
            Validate();

            float volume = Mathf.Log10(value) * 20;
            channel.audioMixer.SetFloat(volumeProperty, volume);
            PlayerPrefs.SetFloat(PlayerPrefsKey, volume);
        }
    }
}