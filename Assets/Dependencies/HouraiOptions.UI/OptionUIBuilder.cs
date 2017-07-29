using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HouraiTeahouse.Options.UI {

    public class OptionUIBuilder : MonoBehaviour, ISerializationCallbackReceiver {

        static readonly ILog _log = Log.GetLogger<OptionUIBuilder>();

        [Serializable]
        public class ViewMapping {
            public string Type;
            public RectTransform Prefab;
        }

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
        ViewMapping[] _mappings;

        Dictionary<Type, RectTransform> _prefabs;
        static readonly Dictionary<Type, AbstractOptionViewAttribute> _defaultViews;

        void RefreshTypes() {
            var viewType = typeof(AbstractOptionViewAttribute);
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                            t => !t.IsAbstract && viewType.IsAssignableFrom(t));
            if (_prefabs == null)
                _prefabs = new Dictionary<Type, RectTransform>();
            foreach(var type in types)
                if (!_prefabs.ContainsKey(type))
                    _prefabs[type] = null;
        }

        void Reset() {
            RefreshTypes();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            RefreshTypes();
            _mappings = _prefabs.Select(kv => new ViewMapping {
                Type = kv.Key.FullName,
                Prefab = kv.Value
            }).ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (_mappings == null)
                return;
            _prefabs = _mappings.Where(m => Type.GetType(m.Type) != null)
                                .ToDictionary(m => Type.GetType(m.Type),
                                              m => m.Prefab);
        }

        static OptionUIBuilder() {
            _defaultViews = new Dictionary<Type, AbstractOptionViewAttribute> {
                {typeof(int), new IntField()},
                {typeof(float), new IntField()},
                {typeof(bool), new Toggle()},
                {typeof(string), new TextField()}
            };
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            var optionsManager = OptionsManager.Instance;
            optionsManager.LoadAllOptions();
            Transform viewContainer = _container;
            if (viewContainer == null)
                viewContainer = transform;
            foreach (CategoryInfo category in optionsManager.Categories) {
                var categoryLabel = Instantiate(categoryLabelTemplate);
                SetNameAndText(categoryLabel.gameObject, category.Name);
                BuildLayout(categoryLabel.gameObject);
                categoryLabel.SetParent(viewContainer, false);
                categoryLabel.gameObject.SetUIText(category.Name);
                foreach (OptionInfo option in category.Options) {
                    var drawer = option.PropertyInfo.GetCustomAttributes(true).OfType<AbstractOptionViewAttribute>().FirstOrDefault();
                    var propertyInfo = option.PropertyInfo;
                    if (drawer == null && !_defaultViews.TryGetValue(propertyInfo.PropertyType, out drawer)) {
                        _log.Error("No drawer and no default drawer can be found for {0} ({1})", 
                            propertyInfo, propertyInfo.PropertyType);
                        continue;
                    }
                    var drawerType = drawer.GetType();
                    RectTransform prefab = null;
                    while (drawerType != null && prefab == null) {
                        _prefabs.TryGetValue(drawerType, out prefab);
                        drawerType = drawerType.BaseType;
                    }
                    if (prefab == null) {
                        _log.Error("No prefab for drawer {0} could be found.", drawerType);
                        continue;
                    }
                    var container = new GameObject(option.Name, typeof(RectTransform));
                    BuildLayout(container);
                    container.transform.SetParent(viewContainer, false);
                    var optionLabel = Instantiate(optionLabelTemplate);
                    var control = Instantiate(prefab);
                    optionLabel.SetParent(container.transform, false);
                    control.SetParent(container.transform, false);
                    SetNameAndText(container, option.Name);
                    FillRect(optionLabel, _indent, _labelSize);
                    FillRect(control, _indent + _labelSize, 1 - (_indent + _labelSize));
                    if (drawer == null)
                        continue;
                    drawer.BuildUI(option, control.gameObject);
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
