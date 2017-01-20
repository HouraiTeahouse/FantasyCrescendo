using HouraiTeahouse.SmashBrew.Matches;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A behaviour used to display a Player's remaining stock </summary>
    //TODO: Change to be event based 
    public class PlayerStockDisplay : MonoBehaviour, IDataComponent<Player> {

        Player _player;

        StockMatch _stockMatch;

        [SerializeField]
        [Tooltip("The Text object used to display the additional stock beyond shown by the simple indicators")]
        NumberText ExcessDisplay;

        [SerializeField]
        [Tooltip("The standard indicators to show current stock values")]
        GameObject[] standardIndicators;

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            if (_player != null)
                _player.Changed -= Refresh;
            _player = data;
            if (_player != null)
                _player.Changed += Refresh;
        }

        /// <summary> Unity Callback. Called before the object's first frame. </summary>
        void Start() {
            _stockMatch = FindObjectOfType<StockMatch>();
            _stockMatch.StockChanged += Refresh;
            DisableCheck();
        }

        void Refresh() {
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
        }

    }

}
