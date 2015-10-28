using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace Hourai {
    
    [AddComponentMenu("")]          //Assure that this cannot be added to a GameObject from the Editor
    internal sealed class TimeBasedAudio : MonoBehaviour {

        private AudioSource _audio;
        
        public AudioSource Audio {
            get { return _audio; }
        }

        public float Pitch { get; set; }

        public bool DestroyObject { get; set; }

        public bool DestroyGameObject { get; set; }

        void Awake() {
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.hideFlags = HideFlags.HideInInspector;
            hideFlags = HideFlags.HideInInspector;
            DestroyObject = true;
            Pitch = 1f;
        }

        void Update() {
            _audio.pitch = Game.TimeScale * Pitch;
            if (DestroyObject && !_audio.isPlaying) {
                if (DestroyGameObject)
                    Destroy(gameObject);
                else
                    Destroy(this);
            }
        }

    }


}

