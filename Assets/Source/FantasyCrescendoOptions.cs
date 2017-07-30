using HouraiTeahouse.Options;
using HouraiTeahouse.Options.UI;
using HouraiTeahouse.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

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

    [OptionCategory("Localization")]
    public class LocalizationOptions {

        [Option("Language"), LanguageSelectDropdown]
        public string GameLanguage { get; set; }

    }

    public class LanguageSelectDropdown : Dropdown {

        readonly string[] _identifiers;

        public LanguageSelectDropdown()  {
            _identifiers = LanguageManager.Instance.AvailableLanguages.ToArray();
            Options = _identifiers.Select(lang => Language.GetName(lang)).ToList();
        }

        public override int GetInitialIndex(string value) {
            return Array.IndexOf(_identifiers, value);
        }

        public override void SaveValue(OptionInfo option, string selectionValue, int index) {
            var manager = LanguageManager.Instance;
            if (manager != null)
                manager.LoadLanguage(_identifiers[index]);
        }

    }

    public class FantasyCrescendoOptions : MonoBehaviour {

        [Serializable]
        public class VolumeSettings {
            [NonSerialized]
            public OptionInfo Option;
            public string Name;
            public float MinDb = -80f;
            public float MaxDb = 10f;

            public float GetDbValue(float normalizedValue) {
                var val = Mathf.Sqrt(Mathf.Sqrt(normalizedValue));
                return Mathf.Lerp(MinDb, MaxDb, val);
            }
        }

        [Header("Audio")]
        [SerializeField]
        AudioMixer _audio;

        [SerializeField]
        string _masterVol = "MasterVolume";

        [SerializeField]
        VolumeSettings[] _volumeChannels;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            var optionsManager = OptionsManager.Instance;
            BuildAudioOptions(optionsManager.GetInfo<AudioOptions>());
            BuildVideoOptions(optionsManager.GetInfo<VideoOptions>());
            var lang = optionsManager.GetInfo<LocalizationOptions>().GetInfo("GameLanguage");
            LanguageManager.Storage = new HouraiOptionLangaugeStorage(lang);
            var manager = LanguageManager.Instance;
            if (manager == null)
                return;
            var storedValue = lang.GetPropertyValue<string>();
            if (storedValue != null && storedValue != "null")
                manager.LoadLanguage(storedValue);
            else {
                Log.Debug(manager.CurrentLangauge.Name);
                lang.SetPropertyValue(manager.CurrentLangauge.Name);
            }
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        void OnApplicationFocus(bool focusStatus) {
            // Mute the game when focus is lost.
            // Unmute when it is regained.
            if (focusStatus) {
                foreach (var channel in _volumeChannels)
                    if (channel.Name == _masterVol && channel.Option != null)
                        _audio.SetFloat(channel.Name, channel.GetDbValue((float)channel.Option.GetPropertyValue()));
            } else {
                foreach (var channel in _volumeChannels)
                    if (channel.Name == _masterVol)
                        _audio.SetFloat(channel.Name, channel.GetDbValue(0f));
            }
        }

        void BuildAudioOptions(CategoryInfo category) {
            foreach(var vol in _volumeChannels) {
                var name = vol.Name;
                vol.Option = category.GetInfo(name);
                ApplyAndListen<float>(vol.Option, val => _audio.SetFloat(name, vol.GetDbValue(val)));
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

    public class HouraiOptionLangaugeStorage : ILanguageStorageDelegate {

        readonly OptionInfo _option;

        public event Action<string> OnChangeLangauge;

        public HouraiOptionLangaugeStorage(OptionInfo option) {
            _option = Argument.NotNull(option);
        }

        public string GetStoredLangaugeId(string defaultLangauge) {
            return _option.GetPropertyValue<string>();
        }

        public void SetStoredLanguageId(string language) {
            if (_option.GetPropertyValue<string>() != language) {
                _option.SetPropertyValue(language);
                _option.Save();
                OnChangeLangauge.SafeInvoke(language);
            }
        }
        
    }

}

