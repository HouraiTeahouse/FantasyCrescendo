using HouraiTeahouse.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace HouraiTeahouse.FantasyCrescendo {
    
public class PostProcessingOptionsControl : MonoBehaviour {

  public PostProcessLayer PostProcessLayer;
  public PostProcessVolume GlobalVolume;
  public Option Antialiasing;
  public Option BloomOption;
  public Option DepthOfFieldOption;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (PostProcessLayer == null) return;
    if (Antialiasing != null) {
      PostProcessLayer.antialiasingMode = Antialiasing.Get<PostProcessLayer.Antialiasing>();
    }
    var profile = GlobalVolume.profile;
    RemoveSettingIfOption<Bloom>(profile, BloomOption);
    RemoveSettingIfOption<DepthOfField>(profile, DepthOfFieldOption);
  }

  void RemoveSettingIfOption<T>(PostProcessProfile profile, Option option) where T : PostProcessEffectSettings {
    if (!option.Get<bool>() && profile.HasSettings<T>()) {
      profile.RemoveSettings<T>();
    }
  }

}

}
