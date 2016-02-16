using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// Static class for editing the global time properties of the game.
    /// Allows for pausing of the game and altering the global time scale.
    /// 
    /// Inherits from MonoBehaviour. A custom Editor allows editing the pause/timescale
    /// state of the game from the Editor.
    /// </summary>
    public class TimeManager : MonoBehaviour {

        private static float _timeScale = 1f;
        private static bool _paused;

        /// <summary>
        /// Gets or sets whether the game is paused or not.
        /// Changing this value will fire the OnPause event
        /// If the value is the same, nothing will change.
        /// 
        /// If the game is paused, Time.timeScale will be set to 0.
        /// When unpaused, Time.timeScale will be set to the value of TimeScale
        /// </summary>
        public static bool Paused {
            get { return _paused; }
            set {
                if (_paused == value)
                    return;
                _paused = value;
                Time.timeScale = _paused ? 0f : TimeScale;
                if (OnPause != null)
                    OnPause();
            }
        }

        /// <summary>
        /// Gets or sets the global timescale of the game.
        /// If the game is not paused, Time.timeScale will also be set to the same value
        /// </summary>
        public static float TimeScale {
            get { return  _timeScale; }
            set {
                if (_timeScale == value)
                    return;
                _timeScale = value;
                if(!Paused)
                    Time.timeScale = value;
                if (OnTimeScaleChange != null)
                    OnTimeScaleChange();
            }
        }

        /// <summary>
        /// Event. Called every time the game is paused or unpaused.
        /// </summary>
        public static event Action OnPause;

        /// <summary>
        /// Event. Called every time the global timescale is changed.
        /// </summary>
        public static event Action OnTimeScaleChange;

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            _timeScale = Time.timeScale;
        }

    }

}
