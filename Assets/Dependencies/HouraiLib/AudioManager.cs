using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Audio;

namespace Hourai {

    public class AudioManager : MonoBehaviour {

        [System.Serializable]
        public class Channel {

            [SerializeField]
            [Tooltip("The viewable name for the channel. May be used for in-game UI elements.")]
            private string _name;

            [SerializeField]
            [Tooltip("The default volume for the channel")]
            private float _baseVolume = 1f;

            [SerializeField]
            [Tooltip("The PlayerPrefs key to save the volume data onto")]
            private string _playerPrefsKey;

            [SerializeField]
            [Tooltip("The associated exposed parameters on the Audio Mixer that are to be changed.")]
            private string[] _associatedParams;

            private AudioMixer _mixer;
            private float _currentVolume;

            /// <summary>
            /// The Channel's current volume. Editing this volume will make it louder or softer.
            /// Range of [0..1].
            /// </summary>
            public float CurrentVolume {
                get { return _currentVolume; }
                set {
                    _currentVolume = value;
                    foreach (string param in _associatedParams)
                        _mixer.SetFloat(param, value);
                }
            }

            /// <summary>
            /// The viewable name for the channel.
            /// </summary>
            public string Name {
                get { return _name; }
                set { _name = value; }
            }

            /// <summary>
            /// Initializes the Channel. Retrieves volume data from PlayerPrefs or sets it to a default value if it doesn't exist.
            /// </summary>
            /// <param name="mixer">the main Audio mixer for the game</param>
            internal void Initialize(AudioMixer mixer) {
                _mixer = mixer;
                if (!Prefs.HasKey(_playerPrefsKey)) {
                    Prefs.SetFloat(_playerPrefsKey, _baseVolume);
                    CurrentVolume = _baseVolume;
                } else {
                    CurrentVolume = Prefs.GetFloat(_playerPrefsKey);
                }
            }

            /// <summary>
            /// Saves the current volume of the channel to PlayerPrefs
            /// </summary>
            internal void Save() {
                Prefs.SetFloat(_playerPrefsKey, _currentVolume);
            }

        }

        [SerializeField]
        private AudioMixer _mixer;

        [SerializeField]
        private Channel[] _channels;

        private ReadOnlyCollection<Channel> _channelCollection;
        private Dictionary<string, Channel> _channelByName;

        /// <summary>
        /// Singleton instance of AudioManager. If null, there does not exist one in the scene.
        /// </summary>
        public static AudioManager Instance { get; private set; }

        /// <summary>
        /// A collection of the Channels defined in the editor.
        /// </summary>
        public ReadOnlyCollection<Channel> Channels {
            get { return _channelCollection; }
        }

        public Channel this[string name] {
            get { return _channelByName[name]; }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            Instance = this;
            _channelCollection = new ReadOnlyCollection<Channel>(_channels);
            _channelByName = new Dictionary<string, Channel>();
            if (_channels == null || _mixer == null)
                return;
            foreach (Channel channel in _channels) {
                if (channel != null) {
                    channel.Initialize(_mixer);
                    _channelByName[channel.Name] = channel;
                }
            }
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            Save();
        }

        /// <summary>
        /// Unity Callback. Called when the entire application is exited.
        /// </summary>
        void OnApplicationQuit() {
            Save();
        }

        /// <summary>
        /// Saves all of the channels to PlayerPrefs to allow for persistence.
        /// </summary>
        void Save() {
            foreach(Channel channel in _channels)
                if(channel != null)
                    channel.Save();
        }
    }

}
