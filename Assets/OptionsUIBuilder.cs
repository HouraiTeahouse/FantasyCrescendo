using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {
    
public class OptionsUIBuilder : MonoBehaviour {

  public List<Option> Options;
  public bool LoadFromResources;

  public RectTransform TabContainer;
  public RectTransform DisplayContainer;

  [Header("Building Blocks")]
  public RectTransform LabelPrefab;
  public Dropdown DropdownPrefab;
  public Toggle TogglePrefab;
  public Slider SliderPrefab;

  [Serializable]
  public struct AdditionalObject {
    public string Category;
    public string Label;
    public RectTransform Prefab;
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (LoadFromResources) {
      LoadOptionsFromResources();
    }
    if (!Debug.isDebugBuild) {
      RemoveDebugOptions();
    }
    WarnDuplicates();
    BuildUI();
  }

  void BuildUI() {
    var groupedOptions = Options.GroupBy(option => option.Category);
    foreach (var group in groupedOptions) {
      var groupTab = BuildGroupUI(group);
      AddTab(group.Key, groupTab);
    }
  }

  RectTransform BuildGroupUI(IEnumerable<Option> options) {
    var orderedOptions = options.OrderBy(option => option.SortOrder).ThenBy(option => option.GetDisplayName());
    foreach (var option in orderedOptions) {

    }
    return TabContainer;
  }

  void CreateOptionUI(Option option) {
    // Create label
    // Create Control
  }

  GameObject GetPrefab(Option option) {
    switch (option.Type) {
      case OptionType.Integer: 
      case OptionType.Float:
        return SliderPrefab.gameObject;
      case OptionType.Boolean:
        return TogglePrefab.gameObject;
      case OptionType.Enum:
        return DropdownPrefab.gameObject;
      default:
        return null;
    }
  }

  void AddTab(string tabName, RectTransform tab) {

  }

  void LoadOptionsFromResources() {
    Options.AddRange(Resources.LoadAll<Option>(string.Empty));
  }

  void RemoveDebugOptions() {
    Options = Options.Where(option => !option.IsDebug).ToList();
  }

  void WarnDuplicates() {
    var set = new HashSet<string>();
    foreach (var option in Options) {
      if (set.Contains(option.Path)) {
        Debug.LogWarning($"Duplicate option for path {option.Path} found: {option}.");
      } else {
        set.Add(option.Path);
      }
    }
  }

}

}