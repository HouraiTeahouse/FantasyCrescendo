using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A custom layout group created for controlling the layout of the individual character select squares on the
    /// character select screen </summary>
    public class CharacterLayoutGroup : LayoutGroup, ILayoutSelfController {

        [SerializeField]
        [Tooltip("The target aspect ratio for the individual children")]
        float _childAspectRatio;

        [SerializeField]
        [Tooltip("The target aspect ratio for the overall layout")]
        float _selfAspectRatio;

        [SerializeField]
        [Tooltip("The spacing between child elements, in pixels")]
        Vector2 _spacing;

        /// <summary>
        ///     <see cref="LayoutGroup.SetLayoutHorizontal" />
        /// </summary>
        public override void SetLayoutHorizontal() {
            SetLayout(true);
        }

        /// <summary>
        ///     <see cref="LayoutGroup.SetLayoutVertical" />
        /// </summary>
        public override void SetLayoutVertical() {
            SetLayout(false);
        }

        /// <summary>
        ///     <see cref="LayoutGroup.CalculateLayoutInputHorizontal" />
        /// </summary>
        public override void CalculateLayoutInputHorizontal() {
            base.CalculateLayoutInputHorizontal();
            SetLayoutInputForAxis(padding.horizontal, padding.horizontal, -1, 0);
        }

        /// <summary>
        ///     <see cref="LayoutGroup.CalculateLayoutInputVertical" />
        /// </summary>
        public override void CalculateLayoutInputVertical() {
            SetLayoutInputForAxis(padding.vertical, padding.vertical, -1, 1);
        }

        /// <summary> Builds the full layout of all children along one axis. </summary>
        /// <param name="axis"> which axis to build on, true if vertical, false if horizontal </param>
        void SetLayout(bool axis) {
            // Givens
            // Child Aspect Ratio
            Vector2 availableSpace = rectTransform.rect.size;
            int count = rectChildren.Count;

            if (availableSpace.x / availableSpace.y > _selfAspectRatio) {
                availableSpace.x = availableSpace.y * _selfAspectRatio;
            }

            // Calculated
            var bestRows = 1;
            var bestCols = 1;
            Vector2 itemSize = Vector2.zero;
            bool isPrime = count <= 3;
            int effectiveCount = count;
            float maxArea = float.MaxValue;

            // Prime numbers tend to generate very poorly made layouts.
            // Repeat until the the layout is generated with a non-prime count
            // Shouldn't need to run more than twice. 
            do {
                for (var rows = 1; rows <= effectiveCount; rows++) {
                    // Reject any "non-rectangle" layouts.
                    if (effectiveCount % rows != 0)
                        continue;
                    isPrime |= rows != 1 && rows != effectiveCount;
                    int cols = effectiveCount / rows;
                    Vector2 effectiveSpace = availableSpace
                        - new Vector2(Mathf.Max(0, cols - 1) * _spacing.x, Mathf.Max(0, rows - 1) * _spacing.y);
                    var size = new Vector2(effectiveSpace.x / cols, effectiveSpace.y / rows);
                    float area = Mathf.Abs(rows * size.x - cols * size.y * _childAspectRatio);
                    if (area >= maxArea)
                        continue;
                    maxArea = area;
                    bestRows = rows;
                    bestCols = cols;
                    itemSize = size;
                }
                effectiveCount++;
            }
            while (!isPrime);

            Vector2 delta = itemSize + _spacing;

            // Only set the sizes when invoked for horizontal axis, not the positions.

            if (axis) {
                foreach (RectTransform rect in rectChildren) {
                    m_Tracker.Add(this,
                        rect,
                        DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition
                            | DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = itemSize;
                }
                return;
            }

            Vector2 center = rectTransform.rect.size / 2;
            Vector2 extents = 0.5f
                * new Vector2(bestCols * itemSize.x + Mathf.Max(0, bestCols - 1) * _spacing.x,
                    bestRows * itemSize.y + Mathf.Max(0, bestRows - 1) * _spacing.y);
            Vector2 start = center - extents;

            for (var i = 0; i < bestRows; i++) {
                float x = start.x;
                float y = start.y + i * delta.y;
                if (count - i * bestCols < bestCols)
                    x = center.x - 0.5f * (count % bestCols * itemSize.x);

                for (var j = 0; j < bestCols; j++) {
                    int index = bestCols * i + j;
                    if (index >= count)
                        break;

                    SetChildAlongAxis(rectChildren[index], 0, x + j * delta.x, itemSize.x);
                    SetChildAlongAxis(rectChildren[index], 1, y, itemSize.y);
                }
            }
        }

    }

}
