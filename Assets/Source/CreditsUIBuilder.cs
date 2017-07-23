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
        float _optionHeight = 40;

        [Header("Subcomponents")]
        [SerializeField]
        RectTransform _container;
        [SerializeField]
        RectTransform categoryLabelTemplate;
        [SerializeField]
        RectTransform optionLabelTemplate;

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
                var categoryLabel = Instantiate(categoryLabelTemplate);
                SetNameAndText(categoryLabel.gameObject, category.Name);
                BuildLayout(categoryLabel.gameObject);
                categoryLabel.SetParent(viewContainer, false);
                categoryLabel.gameObject.SetUIText(category.Name);
                foreach (string contributor in category.Contributors) {
                    var container = new GameObject(contributor, typeof(RectTransform));
                    BuildLayout(container);
                    container.transform.SetParent(viewContainer, false);
                    var optionLabel = Instantiate(optionLabelTemplate);
                    optionLabel.SetParent(container.transform, false);
                    SetNameAndText(container, contributor);
                    FillRect(optionLabel, _indent, _labelSize);
                }
            }
        }

        void SetNameAndText(GameObject label, string text) {
            label.name = text;
            label.gameObject.SetUIText(text);
        } 

        void BuildLayout(GameObject container) {
            var layout = container.GetOrAddComponent<LayoutElement>();
            layout.preferredHeight = _optionHeight;
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
