﻿using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[Serializable]
public class CharacterIndicator : CharacterComponent {

  public Transform TargetTransform;
  public Vector3 PositionBias = new Vector3(0, 1, 0);

  RectTransform Indicator;
  RectTransform CanvasTransform;
  IStateView<PlayerState>[] ViewComponents;

  public override async Task Init(Character character) {
    if (TargetTransform == null) {
      TargetTransform = character.transform;
    }
    var factory = PlayerIndicatorFactory.Instance; 
    if (!character.IsView) return;
    Assert.IsNotNull(factory);
    Indicator = factory.CreateIndicator();
    Assert.IsNotNull(Indicator);
    var config = character.Config;
    ViewComponents = Indicator.GetComponentsInChildren<IStateView<PlayerState>>();
    CanvasTransform = Indicator.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    await Indicator.gameObject.Broadcast<IInitializable<PlayerConfig>>(comp => comp.Initialize(config));
    await Indicator.gameObject.Broadcast<IPlayerComponent>(comp => comp.Initialize(config, true));
  }

  public override void UpdateView(in PlayerState state) {
    if (Indicator == null) return;
    AdjustActiveState(state);
    AdjustIndicatorPosition(state);
    ViewComponents.UpdateView(state);
  }

  void AdjustActiveState(in PlayerState state) {
    if (Indicator.gameObject.activeSelf != state.IsActive) {
      Indicator.gameObject.SetActive(state.IsActive);
    }
  }

  void AdjustIndicatorPosition(in PlayerState state) {
    var camera = CameraController.Instance?.Camera ?? Camera.main;
    Vector3 worldPosition = TargetTransform.position + PositionBias;
    Vector2 viewportPosition = camera.WorldToViewportPoint(worldPosition);
    Vector2 canvasSizeDelta = CanvasTransform.sizeDelta;
    Indicator.anchoredPosition = Vector2.Scale(viewportPosition, canvasSizeDelta) - 0.5f * canvasSizeDelta;
  }

}

}
