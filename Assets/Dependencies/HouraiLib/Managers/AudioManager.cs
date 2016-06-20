// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
        [Tooltip(
            "The associated exposed parameters on the Audio Mixer that are to be changed."
            )]
        string[] _associatedParams;

        float _currentVolume;

        AudioMixer _mixer;

        [SerializeField]
        [Tooltip(
            "The viewable name for the channel. May be used for in-game UI elements."
            )]
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
        internal void Initialize(AudioMixer mixer) {
            _mixer = mixer;
        }
    }

    /// <summary> A singleton wrapper for the master AudioMixer to provide easier programmatic control over defined audio
    /// channels </summary>
    public sealed class AudioManager : Singleton<AudioManager> {
        [SerializeField]
        [Tooltip("The controllable defined channels1")]
        AudioChannel[] _audioChannels;

        Dictionary<string, AudioChannel> _channelByName;

        ReadOnlyCollection<AudioChannel> _channelCollection;

        [SerializeField]
        [Tooltip("The editable audio mixer")]
        AudioMixer _mixer;

        /// <summary> A collection of the Channels defined in the editor. </summary>
        public ReadOnlyCollection<AudioChannel> Channels {
            get { return _channelCollection; }
        }

        public AudioChannel[] AudioChannels {
            get { return _audioChannels; }

            set { _audioChannels = value; }
        }

        /// <summary> Indexer to get the channel corresponding to it's name. </summary>
        /// <param name="name"> the name of the channel </param>
        /// <returns> the corresponding audio channel </returns>
        public AudioChannel this[string name] {
            get { return _channelByName[name]; }
        }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _channelCollection =
                new ReadOnlyCollection<AudioChannel>(AudioChannels);
            _channelByName = new Dictionary<string, AudioChannel>();
            if (AudioChannels == null || _mixer == null)
                return;
            foreach (AudioChannel channel in AudioChannels) {
                if (channel == null)
                    continue;
                channel.Initialize(_mixer);
                _channelByName[channel.Name] = channel;
            }
        }
    }
}
