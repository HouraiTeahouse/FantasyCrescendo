using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public class AlignedGridLayoutGroup : LayoutGroup {

  [SerializeField] protected Vector2 _preferredCellSize = new Vector2(100, 100);
  public Vector2 preferredCellSize { get { return _preferredCellSize; } set { SetProperty(ref _preferredCellSize, value); } }
  
  [SerializeField] protected float _targetAspectRatio = 1f;
  public float aspectRatio { get { return _targetAspectRatio; } set { SetProperty(ref _targetAspectRatio, value); } }

  [SerializeField] protected int _preferredRowCount = 2;
  public int preferredRowCount { get { return _preferredRowCount; } set { SetProperty(ref _targetAspectRatio, value); } }

  [SerializeField] protected Vector2 _spacing = Vector2.zero;
  public Vector2 spacing { get { return _spacing; } set { SetProperty(ref _spacing, value); } }

  public override void CalculateLayoutInputHorizontal() {
    base.CalculateLayoutInputHorizontal();

    int minColumns = 1;
    int preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));

    SetLayoutInputForAxis(
        padding.horizontal + (preferredCellSize.x + spacing.x) * minColumns - spacing.x,
        padding.horizontal + (preferredCellSize.x + spacing.x) * preferredColumns - spacing.x,
        -1, 0);
  }

  public override void CalculateLayoutInputVertical() {
    float width = rectTransform.rect.size.x;
    float availableWidth = width - padding.horizontal + spacing.x + 0.001f;
    float preferredCellCountX = Mathf.Max(1, availableWidth / preferredCellSize.x + spacing.x);
    int preferredRows = Mathf.CeilToInt(rectChildren.Count / (float)preferredCellCountX);

    float delta = preferredCellSize.y + spacing.y;
    float minSpace = padding.vertical + preferredCellSize.y;
    float preferredSpace = padding.vertical + delta * preferredRows - spacing.y;
    SetLayoutInputForAxis(minSpace, preferredSpace, -1, 1);
  }

  public override void SetLayoutHorizontal() => SetCellsAlongAxis(0);

  public override void SetLayoutVertical() => SetCellsAlongAxis(1);

  Vector2 GetCellSize(Vector2 regionSize, 
                      Vector2 preferredSize,
                      int x, int y) {
    var cellX = Mathf.Min(regionSize.x / x, preferredSize.x);
    var cellY = Mathf.Min(regionSize.y / y, preferredSize.y);
    return new Vector2(cellX, cellY);
  }

  static float GetAspectRatio(Vector2 vector) {
    if (vector.y == 0f) return float.NaN;
    return vector.x / vector.y;
  }

  static float Area(Vector2 region) => region.x * region.y;

  void FindOptimalGridSize(Vector2 size, int childCount, 
                           ref Vector2 cellSize, ref int rows, ref int cols) {
    int preferredColCount = Mathf.FloorToInt(childCount / preferredRowCount) + 1;
    if (size.y >= preferredRowCount * preferredCellSize.y &&
        size.x >= preferredColCount * preferredCellSize.x) { 
      cols = preferredColCount;
      rows = preferredRowCount;
      cellSize = preferredCellSize;
      return;
    }

    float minError = float.PositiveInfinity;
    rows = preferredRowCount;
    cols = preferredColCount;

    for (var i = 1; i < childCount; i++) { // Rows
      for (var j = 1; j < childCount; j++) { // Cols
        if (i * j < childCount) continue;
        var cSize = GetCellSize(size, preferredCellSize, j, i);
        var aspect = GetAspectRatio(cSize);
        var error = Mathf.Abs(aspectRatio - aspect);
        if (error > minError) continue;
        minError = error;
        rows = i;
        cols = j;
        cellSize = cSize;
      }
    }
  }

  private void SetCellsAlongAxis(int axis) {
    // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
    // and only vertical values when invoked for the vertical axis.
    // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
    // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
    // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.

    if (axis == 0) {
      // Only set the sizes when invoked for horizontal axis, not the positions.
      for (int i = 0; i < rectChildren.Count; i++) {
        RectTransform rect = rectChildren[i];

        m_Tracker.Add(this, rect,
            DrivenTransformProperties.Anchors |
            DrivenTransformProperties.AnchoredPosition |
            DrivenTransformProperties.SizeDelta);

        rect.anchorMin = Vector2.up;
        rect.anchorMax = Vector2.up;
        rect.sizeDelta = preferredCellSize;
      }
      return;
    }

    Vector2 size = rectTransform.rect.size;
    Vector2 cellSize = Vector2.zero;
    int cellCountX = 0, cellCountY = 0;
    FindOptimalGridSize(size, rectChildren.Count, ref cellSize, ref cellCountY, ref cellCountX);
    Debug.Log($"{cellCountX} {cellCountY}");

    Vector2 requiredSpace = new Vector2(
            cellCountX * preferredCellSize.x + (cellCountX - 1) * spacing.x,
            cellCountY * preferredCellSize.y + (cellCountY - 1) * spacing.y
    );
    Vector2 startOffset = new Vector2(
            GetStartOffset(0, requiredSpace.x),
            GetStartOffset(1, requiredSpace.y)
    );

    Vector2 delta = cellSize + spacing;
    for (var i = 0; i < cellCountY; i++) {
      var rowGridWidth = Mathf.Min(rectChildren.Count - i * cellCountX, cellCountX);
      if (rowGridWidth < 0) break;
      var rowGridDelta = cellCountX - rowGridWidth;
      var extraSpace = rowGridDelta * cellSize.x / 2f;
      for (var j = 0; j < cellCountX; j++) {
        var cellId = i * cellCountX + j;
        if (cellId >= rectChildren.Count) break;
        SetChildAlongAxis(rectChildren[cellId], 0, extraSpace + startOffset.x + delta.x * j, cellSize.x);
        SetChildAlongAxis(rectChildren[cellId], 1, startOffset.y + delta.y * i, cellSize.y);
      }
    }
    for (int i = 0; i < rectChildren.Count; i++) {

    }
  }

}

}