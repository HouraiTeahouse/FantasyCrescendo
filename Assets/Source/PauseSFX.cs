using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HouraiTeahouse.SmashBrew;

namespace HouraiTeahouse.FantasyCrescendo {

    public class PauseSFX : MonoBehaviour {

        [SerializeField]
        AudioClip _pauseSFX;

        [SerializeField]
        AudioClip _unpauseSFX;

        [SerializeField]
        AudioSource _source;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            SmashTimeManager.OnPause += PlayPauseEffect;
            if (_source == null)
                _source = GetComponentInChildren<AudioSource>();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            SmashTimeManager.OnPause -= PlayPauseEffect;
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() {
            _source = GetComponentInChildren<AudioSource>();
        }

        void PlayPauseEffect() {
            if (_source == null)
                return;
            _source.clip = SmashTimeManager.Paused ? _pauseSFX : _unpauseSFX;
            _source.Play();
        }


    }

}
