using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Players {

public class PlayerStockDisplay : MonoBehaviour, IStateView<PlayerState> {

  public TMP_Text ExcessDisplay;
  public GameObject[] standardIndicators;
  public string ExcessFormat = "{0}x";

  int? lastShownExcess;

  public void UpdateView(in PlayerState state) {
    if (!state.IsActive) {
      SetActive(ExcessDisplay.gameObject, false);
      SetActive(false);
      return;
    }

    bool excess = state.Stocks > standardIndicators.Length;
    SetActive(ExcessDisplay.gameObject, excess);
    if (excess) {
      SetExcess(state.Stocks);
      SetActive(1);
    } else {
      SetActive(state.Stocks);
    }
  }

  void SetExcess(int stocks) {
    if (ExcessDisplay == null || stocks == lastShownExcess) return;
    ExcessDisplay.text = string.Format(ExcessFormat, stocks);
    lastShownExcess = stocks;
  }

  void SetActive(bool active) {
    for (var i = 0; i < standardIndicators.Length; i++) {
      SetActive(standardIndicators[i], active);
    }
  }

  void SetActive(int max) {
    for (var i = 0; i < standardIndicators.Length; i++) {
      SetActive(standardIndicators[i], i < max);
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