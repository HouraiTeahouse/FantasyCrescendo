using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {
    
public interface IOptionsStorage {
  bool IsOptionSet(string path);
  void SaveOption(string path, float value);
  float GetOption(string path);
}

public class PlayerPrefsOptionsStorage : IOptionsStorage {
  public bool IsOptionSet(string path) => PlayerPrefs.HasKey(path);

  public void SaveOption(string path, float value) {
    Debug.Log($"Saved Option \"{path}\": {value}");
    PlayerPrefs.SetFloat(path, value);
  }

  public float GetOption(string path) {
    var value = PlayerPrefs.GetFloat(path);
    Debug.Log($"Loaded Option \"{path}\": {value}");
    return value;
  }

}

}