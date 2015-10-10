using UnityEngine;

namespace Hourai.SmashBrew.UI {

    public class PlayerStockIndicator : PlayerGuiComponent<Stock> {

        [SerializeField]
        private NumberText ExcessDisplay;

        [SerializeField]
        private GameObject[] standardIndicators;

        void Update() {
            if (Component == null) {
                if (ExcessDisplay)
                    ExcessDisplay.gameObject.SetActive(false);
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActive(false);
                return;
            }

            int stock = Component.Lives;
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

    }

}
