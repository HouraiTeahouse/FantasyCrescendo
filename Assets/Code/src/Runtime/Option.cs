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

  public static IOptionsStorage Storage;
  static Option() {
    Storage = new PlayerPrefsOptionsStorage();
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
    var readType = typeof(T);

    if (readType == typeof(float)) {
      return Cast<T>(rawValue);;
    } else if (readType == typeof(int)) {
      return Cast<T>((int)rawValue);
    } else if (readType == typeof(bool)) {
      return Cast<T>(rawValue != 0);
    } else if (readType.IsEnum) {
      return (T)Enum.Parse(typeof(T), ((int)rawValue).ToString());
    } else {
      throw new InvalidOperationException($"Cannot laod option with unsupported type: {readType}");
    }
  }

  public void Set<T>(T input, bool save = true) {
    var oldValue = CurrentRawValue;
    var writeType = typeof(T);
    if (writeType == typeof(float)) {
      CurrentRawValue = Cast<float>(input);
    } else if (writeType == typeof(int)) {
      CurrentRawValue = (float)Cast<int>(input);
    } else if (writeType == typeof(bool)) {
      CurrentRawValue = Cast<bool>(input) ? 1.0f : 0.0f;
    } else if (writeType.IsEnum) {
      CurrentRawValue = (float)Cast<int>(input);
    } else {
      throw new InvalidOperationException($"Cannot save option with unsupported type: {writeType}");
    }
    EnforceMinMax();
    if (oldValue != null && CurrentRawValue != oldValue) {
      OnValueChanged.Invoke();
    }
    if (save) {
      Save();
    }
  }

  public void Save() {
    if (!IsLoaded) {
      LoadValue();
    }
    Storage.SaveOption(Path, CurrentRawValue.Value);
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
    if (!Storage.IsOptionSet(Path)) {
      Storage.SaveOption(Path, DefaultValue);
      return DefaultValue;
    }
    return Storage.GetOption(Path);
  }

}

}