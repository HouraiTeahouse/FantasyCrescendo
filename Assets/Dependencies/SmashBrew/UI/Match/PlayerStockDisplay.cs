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

using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> A behaviour used to display a Player's remaining stock </summary>
    //TODO: Change to be event based 
    public class PlayerStockDisplay : MonoBehaviour, IDataComponent<Player> {
        Player _player;

        StockMatch _stockMatch;

        [SerializeField]
        [Tooltip(
            "The Text object used to display the additional stock beyond shown by the simple indicators"
            )]
        NumberText ExcessDisplay;

        [SerializeField]
        [Tooltip("The standard indicators to show current stock values")]
        GameObject[] standardIndicators;

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            _player = data;
        }

        /// <summary> Unity Callback. Called before the object's first frame. </summary>
        void Start() {
            _stockMatch = FindObjectOfType<StockMatch>();
            DisableCheck();
        }

        /// <summary> Unity Callback. Called once per frame. </summary>
        void Update() {
            DisableCheck();

            if (_stockMatch == null || _player == null)
                return;

            int stock = _stockMatch[_player];
            bool excess = stock > standardIndicators.Length;
            if (ExcessDisplay)
                ExcessDisplay.gameObject.SetActive(excess);
            if (excess) {
                if (ExcessDisplay)
                    ExcessDisplay.Number = stock;
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActive(i == 0);
            }
            else {
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActive(i < stock);
            }
        }

        void DisableCheck() {
            if (_stockMatch != null && _stockMatch.enabled || _player == null)
                return;
            if (ExcessDisplay)
                ExcessDisplay.gameObject.SetActive(false);
            foreach (GameObject indicator in standardIndicators.IgnoreNulls())
                indicator.SetActive(false);
            enabled = false;
        }
    }
}
