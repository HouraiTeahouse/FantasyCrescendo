using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.UI {

public class CreditsUIBuilder : MonoBehaviour {
  [Header("Layout")]
  [Range(0f, 1f)] public float LabelSize = 0.35f;
  [Range(0f, 1f)] public float Indent = 0.05f;
  public float CreditHeight = 40;
  public float SpaceHeight = 20;

  [Header("Subcomponents")]
  public RectTransform Container;
  public RectTransform CategoryLabelTemplate;
  public RectTransform OptionLabelTemplate;
  public int MaxContributorsPerLine = 3;

  public CreditsAsset Credits;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start() {
    if (Credits  == null) {
      Debug.LogError("CreditsUIBuilder does not have a proper CreditsAsset to build from.");
      return;
    }
    Transform viewContainer = Container;
    if (viewContainer == null) {
      viewContainer = transform;
    }
    foreach (CreditsAsset.Category category in Credits.Categories) {
      if (category.Contributors.Length <= 0) continue;
      if (category.Contributors.Length == 1) BuildSingleView(category, viewContainer);
      if (category.Contributors.Length > 1) BuildFullView(category, viewContainer);
      var space = new GameObject("Space", typeof(RectTransform));
      BuildLayout(space, SpaceHeight);
      space.transform.SetParent(viewContainer, false);
    }
  }

  async void BuildSingleView(CreditsAsset.Category category, Transform viewContainer) {
      var categoryName = await category.Name.GetLocalizedString();
      var container = new GameObject(categoryName, typeof(RectTransform));
      BuildLayout(container, CreditHeight);
      var categoryLabel = Instantiate(CategoryLabelTemplate);
      categoryLabel.SetParent(container.transform, false);
      FillRect(categoryLabel, Indent, LabelSize);
      SetNameAndText(categoryLabel.gameObject, categoryName);
      foreach (var text in categoryLabel.GetComponentsInChildren<Text>())
          text.alignment = TextAnchor.MiddleLeft;
      container.transform.SetParent(viewContainer, false);
      var optionLabel = Instantiate(OptionLabelTemplate);
      optionLabel.SetParent(container.transform, false);
      SetNameAndText(optionLabel.gameObject, category.Contributors[0]);
      FillRect(optionLabel, LabelSize, (1 - (Indent + LabelSize)));
      foreach (var text in optionLabel.GetComponentsInChildren<Text>())
          text.alignment = TextAnchor.MiddleRight;
  }

  async void BuildFullView(CreditsAsset.Category category, Transform viewContainer) {
    var categoryLabel = Instantiate(CategoryLabelTemplate);
    var categoryName = await category.Name.GetLocalizedString();
    SetNameAndText(categoryLabel.gameObject, categoryName);
    BuildLayout(categoryLabel.gameObject, CreditHeight);
    categoryLabel.SetParent(viewContainer, false);
    SetText(categoryLabel.gameObject, categoryName);
    int segmentCount = 1;
    foreach (var segment in Segment(category.Contributors.OrderBy(s => s), MaxContributorsPerLine)) {
      var count = segment.Length;
      var size = 1 / (float) count;
      var container = new GameObject(categoryName + " " + segmentCount, typeof(RectTransform));
      BuildLayout(container, CreditHeight);
      container.transform.SetParent(viewContainer, false);
      for (var i = 0; i < segment.Length; i++) {
        var optionLabel = Instantiate(OptionLabelTemplate);
        optionLabel.SetParent(container.transform, false);
        SetNameAndText(optionLabel.gameObject, segment[i]);
        FillRect(optionLabel, i * size, size);
      }
      segmentCount++;
    }
  }

  IEnumerable<T[]> Segment<T>(IEnumerable<T> input, int size) {
    var segment = new List<T>();
    foreach (var value in input) {
      segment.Add(value);
      if (segment.Count >= size) {
        yield return segment.ToArray();
        segment.Clear();
      }
    }
    if (segment.Count > 0) {
      yield return segment.ToArray();
    }
  }

  void SetNameAndText(GameObject label, string text) {
    label.name = text;
    SetText(label, text);
  } 

  void SetText(GameObject label, string text) {
    var textComp = label.GetComponentInChildren<Text>();
    if (textComp != null) {
      textComp.text = text;
    }
  }

  void BuildLayout(GameObject container, float height) {
    var layout = container.GetComponent<LayoutElement>();
    if (layout == null) {
      layout = container.AddComponent<LayoutElement>();
    }
    layout.preferredHeight = height;
    layout.flexibleHeight = 0f;
  }

  void FillRect(RectTransform rect, float start, float size) {
    rect.anchorMin = new Vector2(start, 0);
    rect.anchorMax = new Vector2(start + size, 1f);
    rect.offsetMax = Vector2.zero;
    rect.offsetMin = Vector2.zero;
  }

}

}
