using UnityEngine;

namespace Hourai.SmashBrew.UI {

    public class PlayerStockIndicator : MonoBehaviour, IPlayerGUIComponent {

        [SerializeField]
        private NumberText ExcessDisplay;

        [SerializeField]
        private GameObject[] standardIndicators;

        private StockMatch _stockMatch;
        private Player _player;

        void Start() {
            _stockMatch = FindObjectOfType<StockMatch>();
            DisableCheck();
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

        void Update() {
            DisableCheck();

            int stock = _stockMatch[_player];
            bool excess = stock > standardIndicators.Length;
            if(ExcessDisplay)
                ExcessDisplay.gameObject.SetActive(excess);
            if (excess) {
                if (ExcessDisplay)
                    ExcessDisplay.Number = stock;
                for (var i = 0; i < standardIndicators.Length; i++)
                    if(standardIndicators[i])
                        standardIndicators[i].SetActive(i == 0);
            } else {
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActive(i < stock);
            }
        }

        public void SetPlayerData(Player data) {
            _player = data;
        }

    }

}
