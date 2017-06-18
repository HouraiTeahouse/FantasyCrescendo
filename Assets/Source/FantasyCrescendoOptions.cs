using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using HouraiTeahouse.Options;
using HouraiTeahouse.Options.UI;

namespace HouraiTeahouse {

    [OptionCategory("Audio")]
    public class AudioOptions {

        [Option("Master Volume", DefaultValue=(2f/3f)), Slider(0f, 1f)]
        public float MasterVolume { get; set; }

        [Option("BGM Volume", DefaultValue=(2f/3f)), Slider(0f, 1f)]
        public float MusicVolume { get; set; }

        [Option("SFX Volume", DefaultValue=(2f/3f)), Slider(0f, 1f)]
        public float EffectVolume { get; set; }

        [Option("VA Volume", DefaultValue=(2f/3f)), Slider(0f, 1f)]
        public float VoiceVolume { get; set; }

    }

    [OptionCategory("Video")]
    public class VideoOptions {

        [Option, ResolutionDropdown]
        public string Resolution { get; set; }

        [Option, Dropdown("MSAA", "TSAA", "FXAA")]
        public string Antialiasing { get; set; }

    }

    public class FantasyCrescendoOptions : MonoBehaviour {

        [SerializeField]
        AudioMixer _audio;

        [Serializable]
        public class VolumeSettings {
            public string Name;
            public float MinDb = -80f;
            public float MaxDb = 10f;

            public float GetDbValue(float normalizedValue) {
                var val = Mathf.Sqrt(Mathf.Sqrt(normalizedValue));
                return Mathf.Lerp(MinDb, MaxDb, val);
            }
        }

        [SerializeField]
        VolumeSettings[] _volumeChannels;

        void Start() {
            var category = OptionsManager.Instance.GetInfo<AudioOptions>();
            foreach(var vol in _volumeChannels) {
                // TODO(james7132): Use logarithmic change for this
                var name = vol.Name;
                var option = category.GetInfo(name);
                _audio.SetFloat(name, vol.GetDbValue(option.GetPropertyValue<float>()));
                category.GetInfo(name).AddListener<float>((b, a) => {
                    _audio.SetFloat(name, vol.GetDbValue(a));
                });
            }
        }

    }

}

