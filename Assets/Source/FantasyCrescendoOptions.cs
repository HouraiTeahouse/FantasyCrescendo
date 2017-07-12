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

        [Option("Quality Level"), QualityLevelDropdown]
        public string QualityLevel { get; set; }

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
            var optionsManager = OptionsManager.Instance;
            BuildAudioOptions(optionsManager.GetInfo<AudioOptions>());
            BuildVideoOptions(optionsManager.GetInfo<VideoOptions>());
        }

        void BuildAudioOptions(CategoryInfo category) {
            foreach(var vol in _volumeChannels) {
                var name = vol.Name;
                ApplyAndListen<float>(category.GetInfo(name), val => _audio.SetFloat(name, vol.GetDbValue(val)));
            }
        }

        void BuildVideoOptions(CategoryInfo category) {
            ApplyAndListen<string>(category.GetInfo("QualityLevel"), val => {
                var names = QualitySettings.names;
                var index = Array.IndexOf(names, val);
                if (index < 0) {
                    index = names.Length - 1;
                }
                QualitySettings.SetQualityLevel(index, true);
                Log.Info("Quality Level: {0}", QualitySettings.names[QualitySettings.GetQualityLevel()]); 
            });
        }

        void ApplyAndListen<T>(OptionInfo option, Action<T> handler) {
            handler(option.GetPropertyValue<T>());
            option.AddListener<T>((b, a) => handler(a));
        }

    }

}

