using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterIndicator : MonoBehaviour, IPlayerView {

  public Transform TargetTransform;
  public Vector3 PositionBias = new Vector3(0, 1, 0);

  RectTransform Indicator;
  RectTransform CanvasTransform;
  IStateView<PlayerState>[] ViewComponents;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (TargetTransform == null) {
      TargetTransform = transform;
    }
  }

  public async Task Initialize(PlayerConfig config, bool isView = false) {
    var factoryInstance = PlayerIndicatorFactory.Instance; 
    if (!isView || !factoryInstance) return;
    Indicator = factoryInstance.CreateIndicator();
    Assert.IsNotNull(Indicator);
    ViewComponents = Indicator.GetComponentsInChildren<IStateView<PlayerState>>();
    CanvasTransform = Indicator.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    await Indicator.gameObject.Broadcast<IInitializable<PlayerConfig>>(comp => comp.Initialize(config));
    await Indicator.gameObject.Broadcast<IPlayerComponent>(comp => comp.Initialize(config, isView));
  }

  public void ApplyState(PlayerState state) {
    if (Indicator == null) return;
    AdjustActiveState(state);
    AdjustIndicatorPosition(state);
    foreach (var view in ViewComponents) {
      view.ApplyState(state);
    }
  }

  void AdjustActiveState(PlayerState state) {
    if (Indicator.gameObject.activeSelf != state.IsActive) {
      Indicator.gameObject.SetActive(state.IsActive);
    }
  }

  void AdjustIndicatorPosition(PlayerState state) {
    var camera = CameraController.Instance?.Camera ?? Camera.main;
    Vector3 worldPosition = TargetTransform.position + PositionBias;
    Vector2 viewportPosition = camera.WorldToViewportPoint(worldPosition);
    Vector2 canvasSizeDelta = CanvasTransform.sizeDelta;
    Indicator.anchoredPosition = Vector2.Scale(viewportPosition, canvasSizeDelta) - 0.5f * canvasSizeDelta;
  }

}

}
