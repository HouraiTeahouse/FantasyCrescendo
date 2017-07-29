using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

    public class CreditsUIBuilder : MonoBehaviour {

        static readonly ILog _log = Log.GetLogger<CreditsUIBuilder>();

        [Header("Layout")]
        [SerializeField, Range(0f, 1f)]
        float _labelSize = 0.35f;

        [SerializeField, Range(0f, 1f)]
        float _indent = 0.05f;

        [SerializeField]
        float _creditHeight = 40;

        [SerializeField]
        float _spaceHeight = 20;

        [Header("Subcomponents")]
        [SerializeField]
        RectTransform _container;
        [SerializeField]
        RectTransform categoryLabelTemplate;
        [SerializeField]
        RectTransform optionLabelTemplate;

        [SerializeField]
        int _maxContributorsPerLine = 3;

        [SerializeField]
        CreditsAsset _credits;

        Dictionary<Type, RectTransform> _prefabs;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            if (_credits == null) {
                _log.Error("CreditsUIBuilder does not have a proper CreditsAsset to build from.");
                return;
            }
            Transform viewContainer = _container;
            if (viewContainer == null)
                viewContainer = transform;
            foreach (CreditsAsset.Category category in _credits.Categories) {
                if (category.Contributors.Length <= 0)
                    continue;
                if (category.Contributors.Length == 1)
                    BuildSingleView(category, viewContainer);
                if (category.Contributors.Length > 1) 
                    BuildFullView(category, viewContainer);
                var space = new GameObject("Space", typeof(RectTransform));
                BuildLayout(space, _spaceHeight);
                space.transform.SetParent(viewContainer, false);
            }
        }

        void BuildSingleView(CreditsAsset.Category category, Transform viewContainer) {
            var container = new GameObject(category.Name, typeof(RectTransform));
            BuildLayout(container, _creditHeight);
            var categoryLabel = Instantiate(categoryLabelTemplate);
            categoryLabel.SetParent(container.transform, false);
            FillRect(categoryLabel, _indent, _labelSize);
            SetNameAndText(categoryLabel.gameObject, category.Name);
            foreach (var text in categoryLabel.GetComponentsInChildren<Text>())
                text.alignment = TextAnchor.MiddleLeft;
            container.transform.SetParent(viewContainer, false);
            var optionLabel = Instantiate(optionLabelTemplate);
            optionLabel.SetParent(container.transform, false);
            SetNameAndText(optionLabel.gameObject, category.Contributors[0]);
            FillRect(optionLabel, _labelSize, (1 - (_indent + _labelSize)));
            foreach (var text in optionLabel.GetComponentsInChildren<Text>())
                text.alignment = TextAnchor.MiddleRight;
        }

        void BuildFullView(CreditsAsset.Category category, Transform viewContainer) {
            var categoryLabel = Instantiate(categoryLabelTemplate);
            SetNameAndText(categoryLabel.gameObject, category.Name);
            BuildLayout(categoryLabel.gameObject, _creditHeight);
            categoryLabel.SetParent(viewContainer, false);
            categoryLabel.gameObject.SetUIText(category.Name);
            int segmentCount = 1;
            foreach (var segment in Segment(category.Contributors.OrderBy(s => s), _maxContributorsPerLine)) {
                var count = segment.Length;
                var size = 1 / (float) count;
                var container = new GameObject(category.Name + " " + segmentCount, typeof(RectTransform));
                BuildLayout(container, _creditHeight);
                container.transform.SetParent(viewContainer, false);
                for (var i = 0; i < segment.Length; i++) {
                    var optionLabel = Instantiate(optionLabelTemplate);
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
            if (segment.Count > 0)
                yield return segment.ToArray();
        }

        void SetNameAndText(GameObject label, string text) {
            label.name = text;
            label.gameObject.SetUIText(text);
        } 

        void BuildLayout(GameObject container, float height) {
            var layout = container.GetOrAddComponent<LayoutElement>();
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
