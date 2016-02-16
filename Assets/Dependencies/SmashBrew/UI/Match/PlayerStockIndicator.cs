using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerStockIndicator : MonoBehaviour, IPlayerGUIComponent {

        [SerializeField]
        private NumberText ExcessDisplay;

        [SerializeField]
        private GameObject[] standardIndicators;

        private StockMatch _stockMatch;
        private Player _player;

        /// <summary>
        /// Unity Callback. Called before the object's first frame.
        /// </summary>
        void Start() {
            _stockMatch = FindObjectOfType<StockMatch>();
            DisableCheck();
        }

        /// <summary>
        /// Unity Callback. Called once per frame.
        /// </summary>
        void Update() {
            DisableCheck();

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
            } else {
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
            foreach (var indicator in standardIndicators)
                if (indicator)
                    indicator.SetActive(false);
            enabled = false;
        }

        public void SetPlayer(Player data) {
            _player = data;
        }

    }

}
