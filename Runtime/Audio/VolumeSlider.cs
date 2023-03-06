using UnityEngine;
using UnityEngine.UI;

namespace GGL.Audio
{
    /// <summary>
    /// Bind a slider input to a channel and the local storage
    /// </summary>
    [AddComponentMenu(Settings.NAME + "/UI/Audio/Volume Slider")]
    [RequireComponent(typeof(Slider))]
    [HelpURL(Settings.ARTICLE_URL + "audio" + Settings.COMMON_EXT + "#interaction")]
    public class VolumeSlider : VolumeBinder
    {
        private Slider _slider;

        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();
            CheckSlider();
        }


        /// <summary>
        /// Read the local storage to initialize the channel volume and the slider.
        /// This method is called once the component is enabled.
        /// </summary>
        /// <remarks>If not enabled, do not forget to call this method once the scene is loaded.</remarks>
        public override void Init()
        {
            base.Init();
            CheckSlider();
            _slider.value = Value;
        }

        /// <summary>
        /// Try to get and listen the input.
        /// </summary>
        private void CheckSlider()
        {
            if (_slider) return;
            _slider = GetComponent<Slider>();
            if (!_slider) return;
            _slider.minValue = MIN_VOLUME;
            _slider.maxValue = MAX_VOLUME;
            _slider.onValueChanged.AddListener(Set);
        }
    }
}