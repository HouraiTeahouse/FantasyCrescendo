using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerStockDisplay : MonoBehaviour, IStateView<PlayerState> {

  public Text ExcessDisplay;
  public GameObject[] standardIndicators;
  public string ExcessFormat = "{0}x";

  int? lastShownExcess;

  public void ApplyState(PlayerState state) {
    if (!state.IsActive) {
      SetActive(ExcessDisplay.gameObject, false);
      SetActive(i => false);
      return;
    }

    bool excess = state.Stocks > standardIndicators.Length;
    SetActive(ExcessDisplay.gameObject, excess);
    if (excess) {
      SetExcess(state.Stocks);
      SetActive(i => i == 0);
    } else {
      SetActive(i => i < state.Stocks);
    }
  }

  void SetExcess(int stocks) {
    if (ExcessDisplay == null || stocks == lastShownExcess) return;
    ExcessDisplay.text = string.Format(ExcessFormat, stocks);
    lastShownExcess = stocks;
  }

  void SetActive(Func<int, bool> activeFunc) {
    for (var i = 0; i < standardIndicators.Length; i++) {
      SetActive(standardIndicators[i], activeFunc(i));
    }
  }

  void SetActive(GameObject gameObj, bool active) {
    if (gameObj == null) return;
    if (gameObj.activeInHierarchy != active) {
      gameObj.SetActive(active);
    }
  }

}

}