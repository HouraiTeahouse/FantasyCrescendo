using HouraiTeahouse.SmashBrew.Matches;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A behaviour used to display a Player's remaining stock </summary>
    //TODO: Change to be event based 
    public class PlayerStockDisplay : PlayerUIComponent {

        [SerializeField]
        StockMatch _stockMatch;

        [SerializeField]
        [Tooltip("The Text object used to display the additional stock beyond shown by the simple indicators")]
        NumberText ExcessDisplay;

        [SerializeField]
        [Tooltip("The standard indicators to show current stock values")]
        GameObject[] standardIndicators;

        /// <summary> Unity Callback. Called before the object's first frame. </summary>
        protected override void Start() {
            base.Start();
            Refresh();
        }

        void Update() {
            if (_stockMatch != null)
                return;
            _stockMatch = FindObjectOfType<StockMatch>();
            if(_stockMatch != null) {
                _stockMatch.StockChanged += Refresh;
                Refresh();
            }
        }

        void Refresh() {
            if (_stockMatch == null || !_stockMatch.IsActive || Player == null) {
                ExcessDisplay.gameObject.SetActive(false);
                foreach (GameObject standardIndicator in standardIndicators)
                    standardIndicator.SetActive(false);
                return;
            }

            int stock = _stockMatch[Player];
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

    }

}
