using System;
using UnityEngine;
using UnityEngine.Events;

namespace HouraiTeahouse {

public enum OptionType {
  Integer, Float, Boolean, Enum
}

[Serializable]
public struct EnumOption {
  public int Value;
  public string DisplayName;
}

[CreateAssetMenu]
public class Option : ScriptableObject {

  static IOptionsStorage OptionsStorage;
  static Option() {
    OptionsStorage = new PlayerPrefsOptionsStorage();
  }

  public string Path;
  public OptionType Type;

  public float MinValue;
  public float MaxValue;
  public float DefaultValue;

  public string Category;
  public string DisplayName;
  public int SortOrder;

  public bool IsDebug;

  public string EnumType;
  public EnumOption[] EnumOptions;

  private float? CurrentRawValue;
  private bool IsLoaded => CurrentRawValue != null;

  public UnityEvent OnValueChanged;

  public string GetDisplayName() => string.IsNullOrEmpty(DisplayName) ? Path : DisplayName;

  public T Get<T>() {
    var rawValue = GetRawValue();
    switch (Type) {
      case OptionType.Float: 
        return Cast<T>(rawValue);
      case OptionType.Integer: 
      case OptionType.Enum:
        return Cast<T>((int)rawValue);
      case OptionType.Boolean:
        return Cast<T>(rawValue != 0);
      default:
        throw new InvalidOperationException($"Cannot laod option with undefined type: {(int)Type}");
    }
  }

  public void Set<T>(T input, bool save = true) {
    Debug.LogWarning(input);
    switch (Type) {
      case OptionType.Float: 
        CurrentRawValue = Cast<float>(input);
        break;
      case OptionType.Integer: 
      case OptionType.Enum:
        CurrentRawValue = (float)Cast<int>(input);
        break;
      case OptionType.Boolean:
        CurrentRawValue = Cast<bool>(input) ? 1.0f : 0.0f;
        break;
      default:
        throw new InvalidOperationException($"Cannot set option with undefined type: {(int)Type}");
    }
    EnforceMinMax();
    if (save) {
      Save();
    }
  }

  public void Save() {
    if (!IsLoaded) {
      LoadValue();
    }
    OptionsStorage.SaveOption(Path, CurrentRawValue.Value);
  }

  T Cast<T>(object source) => (T)Convert.ChangeType(source, typeof(T));

  float? GetRawValue() {
    if (!IsLoaded)  {
      CurrentRawValue = LoadValue();
    }
    return CurrentRawValue.Value;
  }

  void EnforceMinMax() {
    if (!IsLoaded) return;
    CurrentRawValue = Mathf.Clamp(CurrentRawValue.Value, MinValue, MaxValue);
  }

  float LoadValue() {
    if (!OptionsStorage.IsOptionSet(Path)) {
      OptionsStorage.SaveOption(Path, DefaultValue);
      return DefaultValue;
    }
    return OptionsStorage.GetOption(Path);
  }

}

}