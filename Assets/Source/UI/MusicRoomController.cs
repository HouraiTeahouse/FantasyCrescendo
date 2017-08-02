using HouraiTeahouse.SmashBrew;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    public class MusicRoomController : MonoBehaviour {

        BGMData[] _bgmData;

        [SerializeField]
        PlayBGM _bgmPlayer;

        [SerializeField]
        GameObject _textSource;

        int _currentIndex;

        public event Action<BGMData> OnBGMChange;

        public BGMData CurrentSelectedBGM {
            get { return _bgmData[_currentIndex]; }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (DataManager.Scenes == null)
                _bgmData = new BGMData[0];
            else
                _bgmData = DataManager.Scenes.Where(scene => scene.IsSelectable).SelectMany(scene => scene.MusicData).ToArray();
            SetViewText();
        }

        public void ChangeMusic(int distance) {
            _currentIndex = (_currentIndex + distance) % _bgmData.Length;
            while(_currentIndex < 0)
                _currentIndex += _bgmData.Length;
            OnBGMChange.SafeInvoke(CurrentSelectedBGM);
            SetViewText();
        }

        public void PlayCurrent() {
            Log.Debug(CurrentSelectedBGM);
            _bgmPlayer.Play(CurrentSelectedBGM);
        }

        void SetViewText() {
            if (_textSource == null)
                return;
            var bgm = CurrentSelectedBGM;
            var text = "{0}\nby {1}".With(bgm.Name, bgm.Artist);
            if (!string.IsNullOrEmpty(bgm.OriginalName))
                text += "\nOriginal: " + bgm.OriginalName;
            _textSource.SetUIText(text);
        }

    }

}
