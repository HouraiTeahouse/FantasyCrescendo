using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HouraiTeahouse.Options;
using HouraiTeahouse.Options.UI;

namespace HouraiTeahouse {

    [OptionCategory("Audio")]
    public class AudioOptions {

        [Option("Master Volume"), Slider(0f, 1f)]
        public float MasterVolume { get; set; }

        [Option("BGM Volume"), Slider(0f, 1f)]
        public float MusicVolume { get; set; }

        [Option("SFX Volume"), Slider(0f, 1f)]
        public float EffectVolume { get; set; }

        [Option("VA Volume"), Slider(0f, 1f)]
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
        AudioSource _audio;

        void Awake() {
            var category = _optionSystem.GetInfo<AudioOptions>();
            category.GetInfo("MasterVolume").AddListener<float>((b, a) => {
                _audio.volume = a;
            });
        }

    }

}

