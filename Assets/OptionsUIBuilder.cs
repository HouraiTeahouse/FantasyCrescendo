using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HouraiTeahouse {
    
public class OptionsUIBuilder : MonoBehaviour {

  public List<Option> Options;
  public bool LoadFromResources;

  [Range(0f, 1f)] public float LabelWidth = 0.3f;
  [Range(0f, 1f)] public float HorizontalPadding = 0.05f;
  [Range(0f, 1f)] public float VerticalPadding = 0.025f;

  public RectTransform TabContainer;
  public RectTransform DisplayContainer;

  public RectTransform RowPrefab;
  public RectTransform GroupPrefab;
  public RectTransform TabPrefab;
  public TextMeshProUGUI LabelPrefab;
  public RectTransform DropdownPrefab;
  public RectTransform TogglePrefab;
  public RectTransform SliderPrefab;

  public string GroupInjectionPoint;

  public OptionMenuElement[] AdditionalObjects;

  [Serializable]
  public struct OptionMenuElement {
    public string Category;
    public string Label;
    public int SortPriority;
    public RectTransform Object;
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
    var optionElements = Options.Select<Option, OptionMenuElement>(CreateElement);
    var additionalElements = AdditionalObjects.Select<OptionMenuElement, OptionMenuElement>(CreateElement);
    optionElements = optionElements.Concat(additionalElements);
    var groupedElements = optionElements.Where(elem => elem.Object != null)
                                        .GroupBy(elem => elem.Category);
    foreach (var group in groupedElements) {
      var groupTab = BuildGroupUI(group);
      AddTab(group.Key, groupTab);
    }
  }

  OptionMenuElement CreateElement(Option option) {
    var element = new OptionMenuElement();
    var controlPrefab = GetPrefab(option);
    if (controlPrefab == null) return element;
    element.Category = option.Category;
    element.SortPriority = option.SortOrder;
    element.Object = GetOrCreate(RowPrefab);
    // Create label
    var label = CreateLabel(option.GetDisplayName());
    // Create Control
    var control = Instantiate(controlPrefab).GetComponent<RectTransform>();
    LayoutLabelAndControl(element.Object, label, control);
    return element;
  }

  OptionMenuElement CreateElement(OptionMenuElement obj) {
    var element = obj;
    if (obj.Object == null) return element;

    var control = Instantiate(obj.Object);

    if (string.IsNullOrEmpty(obj.Label)) {
      element.Object = control;
    } else {
      element.Object = GetOrCreate(RowPrefab);
      var label = CreateLabel(obj.Label);
      LayoutLabelAndControl(element.Object, label, control);
    }
    return element;
  }

  RectTransform CreateLabel(string labelText) {
    var label = GetOrCreate(LabelPrefab);
    label.text = labelText;
    return label.GetComponent<RectTransform>();
  }

  void LayoutLabelAndControl(RectTransform container, 
                             RectTransform label, 
                             RectTransform control) {
    if (container == null) return;
    var divider = HorizontalPadding + LabelWidth;
    if (label != null) {
      label.SetParent(container, false);
      SetByAnchor(control, HorizontalPadding, divider, VerticalPadding, 1 - VerticalPadding);
    }
    if (control != null) {
      control.SetParent(container, false);
      SetByAnchor(control, divider, 1 - HorizontalPadding, VerticalPadding, 1 - VerticalPadding);
    }
  }

  T GetOrCreate<T>(T obj) where T : Component {
    if (obj != null) return Instantiate(obj);
    return new GameObject(typeof(T).Name).AddComponent<T>();
  }

  void SetByAnchor(RectTransform transform, 
                   float minX = 0.0f, float maxX = 1.0f,
                   float minY = 0.0f, float maxY = 1.0f) {
    transform.anchorMin = new Vector2(minX, minY);
    transform.anchorMax = new Vector2(maxX, maxY);
    transform.offsetMin = Vector2.zero;
    transform.offsetMax = Vector2.zero;
  }

  RectTransform BuildGroupUI(IEnumerable<OptionMenuElement> options) {
    var orderedOptions = options.OrderBy(option => option.SortPriority)
                                .ThenBy(option => option.Label);
    var container = GetOrCreate(GroupPrefab);
    container.SetParent(DisplayContainer, false);
    SetByAnchor(container);
    var injectionPoint = container.GetComponentsInChildren<RectTransform>()
                                  .FirstOrDefault(obj => obj.name == GroupInjectionPoint);
    if (injectionPoint == null) {
      injectionPoint = container;
    }
    foreach (var option in orderedOptions) {
      option.Object.SetParent(injectionPoint, false);
    }
    return container;
  }

  GameObject GetPrefab(Option option) {
    if (option == null) return null;
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

  void AddTab(string tabName, RectTransform groupContainer) {
    var root = Instantiate(TabPrefab);
    var instance = root.GetComponentInChildren<TextMeshProUGUI>();
    instance.text = tabName;
    root.SetParent(TabContainer, false);
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