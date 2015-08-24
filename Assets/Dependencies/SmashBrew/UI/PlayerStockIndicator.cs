using UnityEngine;
using System.Collections;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew.UI {

    public class PlayerStockIndicator : CharacterGUIComponent<Stock> {

        [Serialize, Show]
        private NumberText ExcessDisplay;

        [Serialize, Show]
        private GameObject[] standardIndicators;

        void Update() {
            if (Component == null) {
                if (ExcessDisplay)
                    ExcessDisplay.gameObject.SetActiveIfNot(false);
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActiveIfNot(false);
                return;
            }

            int stock = Component.Lives;
            bool excess = stock > standardIndicators.Length;
            if(ExcessDisplay)
                ExcessDisplay.gameObject.SetActiveIfNot(excess);
            if (excess) {
                if (ExcessDisplay)
                    ExcessDisplay.Number = stock;
                for (var i = 0; i < standardIndicators.Length; i++)
                    if(standardIndicators[i])
                        standardIndicators[i].SetActiveIfNot(i == 0);
            } else {
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActiveIfNot(i < stock);
            }
        }

    }

}
