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

    public class OptionTest : MonoBehaviour {

        [SerializeField]
        OptionSystem _optionSystem;

        [SerializeField]
        AudioMixer _audio;

        [Serializable]
        public class VolumeSettings {
            public string Name;
            public float MinDb = -30f;
            public float MaxDb = 10f;

            public float GetDbValue(float normalizedValue) {
                return Mathf.Lerp(MinDb, MaxDb, normalizedValue);
            }
        }

        [SerializeField]
        VolumeSettings[] _volumeChannels;

        void Start() {
            var category = _optionSystem.GetInfo<AudioOptions>();
            foreach(var vol in _volumeChannels) {
                // TODO(james7132): Use logarithmic change for this
                var name = vol.Name;
                var option = category.GetInfo(name);
                _audio.SetFloat(name, vol.GetDbValue(option.GetPropertyValue<float>()));
                category.GetInfo(name).AddListener<float>((b, a) => {
                    Log.Debug("{0}, {1}", a, vol.GetDbValue(a));
                    _audio.SetFloat(name, vol.GetDbValue(a));
                });
            }
        }

    }

}

