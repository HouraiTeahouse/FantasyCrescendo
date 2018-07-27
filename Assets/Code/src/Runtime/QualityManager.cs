using HouraiTeahouse.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class QualityManager : MonoBehaviour {

  public Option QualityLevelOption;
  public Option TextureQualityOption;

  [SerializeField]
  bool _bypassTextureQuality;
  public bool BypassTextureQuality {
    get { return _bypassTextureQuality; } 
    set {
      _bypassTextureQuality = value;
      RefreshTextureQuality();
    }
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    BuildQualityLevelOption();
    BuildTextureQualityOption();
    RefreshTextureQuality();
  }

  void BuildQualityLevelOption() {
    var names = QualitySettings.names;
    var options = new EnumOption[names.Length];
    for (var i = 0; i < names.Length; i++) {
      options[i] = new EnumOption {
        Value = i,
        DisplayName = names[i]
      };
    }

    Array.Reverse(options);
    
    // Create a fake enum option
    QualityLevelOption.Type = OptionType.Enum;
    QualityLevelOption.EnumOptions = options;
    QualityLevelOption.MinValue = 0;
    QualityLevelOption.MaxValue = Mathf.Max(0, names.Length - 1);
    QualityLevelOption.DefaultValue = QualitySettings.GetQualityLevel();

    QualitySettings.SetQualityLevel(QualityLevelOption.Get<int>());
    QualityLevelOption.OnValueChanged.AddListener(() => {
      QualitySettings.SetQualityLevel(QualityLevelOption.Get<int>());
    });
  }

  void BuildTextureQualityOption() {
    var names = new[] { "High", "Medium", "Low", "Very Low" };
    var options = new EnumOption[names.Length];
    for (var i = 0; i < names.Length; i++) {
      options[i] = new EnumOption {
        Value = i,
        DisplayName = names[i]
      };
    }
    
    // Create a fake enum option
    TextureQualityOption.Type = OptionType.Enum;
    TextureQualityOption.EnumOptions = options;
    TextureQualityOption.MinValue = 0;
    TextureQualityOption.MaxValue = Mathf.Max(0, names.Length - 1);
    TextureQualityOption.DefaultValue = Mathf.FloorToInt(TextureQualityOption.MaxValue / 2);

    QualitySettings.masterTextureLimit = QualityLevelOption.Get<int>();
    TextureQualityOption.OnValueChanged.AddListener(RefreshTextureQuality);
  }

  void RefreshTextureQuality() {
    if (_bypassTextureQuality) {
      QualitySettings.masterTextureLimit = 0;
    } else {
      QualitySettings.masterTextureLimit = TextureQualityOption.Get<int>();
    }
  }

}

}

