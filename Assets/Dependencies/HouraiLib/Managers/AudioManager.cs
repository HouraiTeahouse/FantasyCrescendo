using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Audio;

namespace HouraiTeahouse {

    /// <summary> A controllable audio channel </summary>
    [Serializable]
    public sealed class AudioChannel {

        [SerializeField]
        [Tooltip("The associated exposed parameters on the Audio Mixer that are to be changed.")]
        string[] _associatedParams;

        float _currentVolume;

        AudioMixer _mixer;

        [SerializeField]
        [Tooltip("The viewable name for the channel. May be used for in-game UI elements.")]
        string _name;

        [SerializeField]
        [Tooltip("The PlayerPrefs key to save the volume data onto")]
        PrefFloat _volume;

        /// <summary> Gets or sets the AudioChannel's current volume. Editing this volume will make it louder or softer. Range of
        /// [0..1]. </summary>
        public float CurrentVolume {
            get { return _currentVolume; }
            set {
                _currentVolume = value;
                foreach (string param in _associatedParams)
                    _mixer.SetFloat(param, value);
            }
        }

        /// <summary> Gets the viewable name for the channel. </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary> Initializes the AudioChannel. Retrieves volume data from PlayerPrefs or sets it to a default value if it
        /// doesn't exist. </summary>
        /// <param name="mixer"> the main Audio mixer for the game </param>
        internal void Initialize(AudioMixer mixer) { _mixer = mixer; }

    }

    /// <summary> A singleton wrapper for the master AudioMixer to provide easier programmatic control over defined audio
    /// channels </summary>
    public sealed class AudioManager : Singleton<AudioManager> {

        [SerializeField]
        [Tooltip("The controllable defined channels1")]
        AudioChannel[] _audioChannels;

        [SerializeField]
        [Tooltip("The editable audio mixer")]
        AudioMixer _mixer;

        Dictionary<string, AudioChannel> _channelByName;

        /// <summary> A collection of the Channels defined in the editor. </summary>
        public ReadOnlyCollection<AudioChannel> Channels { get; private set; }

        /// <summary> Indexer to get the channel corresponding to it's name. </summary>
        /// <param name="channelName"> the name of the channel </param>
        /// <returns> the corresponding audio channel </returns>
        public AudioChannel this[string channelName] {
            get { return _channelByName[channelName]; }
        }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            Channels = new ReadOnlyCollection<AudioChannel>(_audioChannels);
            _channelByName = new Dictionary<string, AudioChannel>();
            if (_audioChannels == null || _mixer == null)
                return;
            foreach (AudioChannel channel in _audioChannels) {
                if (channel == null)
                    continue;
                channel.Initialize(_mixer);
                _channelByName[channel.Name] = channel;
            }
        }

    }

}