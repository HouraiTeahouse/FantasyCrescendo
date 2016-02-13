using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class CharacterLayoutGroup : LayoutGroup, ILayoutSelfController {

        [SerializeField]
        private float _selfAspectRatio;

        [SerializeField]
        private float _childAspectRatio;

        public override void CalculateLayoutInputHorizontal() {
            base.CalculateLayoutInputHorizontal();
            SetLayoutInputForAxis(padding.horizontal, padding.horizontal, -1, 0);
        }

        public override void CalculateLayoutInputVertical() {
            SetLayoutInputForAxis(padding.vertical, padding.vertical, -1, 1);
        }

        public override void SetLayoutHorizontal() {
            SetLayout(true);
        }

        public override void SetLayoutVertical() {
            SetLayout(false);
        }

        private void SetLayout(bool axis) {
            // Givens
            // Child Aspect Ratio
            Vector2 availableSpace = rectTransform.rect.size;
            int count = rectChildren.Count;

            if (availableSpace.x / availableSpace.y > _selfAspectRatio) {
                availableSpace.x = availableSpace.y*_selfAspectRatio;
            }

            // Calculated
            int bestRows = 1;
            int bestCols = 1;
            float maxArea = float.MaxValue;
            float itemWidth = 0;
            float itemHeight = 0;
            for (var rows = 1; rows <= count; rows++) {
                int cols = count / rows;
                int effectiveRows = rows;
                if (count % rows != 0) {
                    effectiveRows++;
                }
                var width = availableSpace.x / cols;
                var height = availableSpace.y / effectiveRows;
                if (width*cols > availableSpace.x) {
                    Debug.Log("Horizontal Problem");
                    continue;
                }
                if (height*effectiveRows > availableSpace.y) {
                    Debug.Log("Vertical Problem " + (height * effectiveRows - availableSpace.y));
                    continue;
                }
                float area = Mathf.Abs(width - height);
                if (area < maxArea) {
                    maxArea = area;
                    bestRows = effectiveRows;
                    bestCols = cols;
                    itemWidth = width;
                    itemHeight = height;
                }
            }

            // Only set the sizes when invoked for horizontal axis, not the positions.

            if (axis) {
                for (int i = 0; i < rectChildren.Count; i++) {
                    RectTransform rect = rectChildren[i];

                    m_Tracker.Add(this, rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = new Vector2(itemWidth, itemHeight);
                }
                return;
            }

            Vector2 center = rectTransform.rect.size / 2;
            Vector2 extents = 0.5f * new Vector2(bestCols * itemWidth, bestRows * itemHeight);
            Vector2 start = center - extents;

            for(var i = 0; i < count; i++) {
                int x = i % bestCols;
                int y = i / bestCols;
                SetChildAlongAxis(rectChildren[i], 0, start.x + x * itemWidth, itemWidth);
                SetChildAlongAxis(rectChildren[i], 1, start.y + y * itemHeight, itemHeight);
            }
        }
    }
}
