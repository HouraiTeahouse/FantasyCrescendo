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

using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> Handler MonoBehaviour that listens for button presses and pauses the game as needed. </summary>
    public class PauseGame : MonoBehaviour {
        /// <summary> The button that pauses the game. </summary>
        [SerializeField]
        InputTarget _pauseButton = InputTarget.Start;

        /// <summary> The player that paused the game. </summary>
        Player _pausedPlayer;

        /// <summary> Unity callback. Called once every frame. </summary>
        void Update() {
            if (TimeManager.Paused) {
                if (_pausedPlayer != null
                    && !_pausedPlayer.Controller.GetControl(_pauseButton)
                        .WasPressed)
                    return;
                _pausedPlayer = null;
                TimeManager.Paused = false;
            }
            else {
                foreach (Player player in Player.ActivePlayers) {
                    if (player.Controller == null
                        || !player.Controller.GetControl(_pauseButton)
                            .WasPressed)
                        continue;
                    _pausedPlayer = player;
                    TimeManager.Paused = true;
                    break;
                }
            }
        }
    }
}