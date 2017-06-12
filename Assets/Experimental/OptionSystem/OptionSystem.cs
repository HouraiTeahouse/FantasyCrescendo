using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

namespace HouraiTeahouse.Options {

    public interface IParser {
        object Parse(string obj);
    }

    public class SimpleParser<T> : IParser {
        readonly Func<string, T> _parseFunc;
        public SimpleParser(Func<string, T> parseFun) { _parseFunc = parseFun; }
        public object Parse(string obj) { return _parseFunc(obj); }
    }

    public class OptionSystem : MonoBehaviour {
        readonly Dictionary<Type, CategoryInfo> _categories = new Dictionary<Type, CategoryInfo>();
            
        public IEnumerable<CategoryInfo> Categories {
            get { return _categories.Values.Select(x => x); }
        }

        public IEnumerable<OptionInfo> Options {
            get { return _categories.Values.SelectMany(c => c.Options); }
        }

        // in registry there will be an entry under this key,
        // which includes all options' key.
        // This exists solely because Unity does not have API
        // to traverse through all PlayerPrefs.
        // This is used for removing obsolete options.

        const string allOptionsKey = "OptionNames";

        // A version tracker for deleting old registries
        // only updates when serialized data format becomes different
        // for example, when PlayerPrefs value changed from "type,value" to "value" only
        const int optionVersion = 1;
        const char keySeperator = ',';
        internal const char optionSeperator = '*';
        const string optionVersionKey = "OptionVersion";

        // OptionSystem's initialization must be done before the MetadataList call
        void Awake() {
            Initialize();
        }

#if UNITY_EDITOR
        // Debug call for saving changes,
        // just for testing purposes
        void Update() {
            if (Input.GetKeyDown(KeyCode.S)) {
                SaveAllChanges();
            }
        }
#endif

        void OnApplicationQuit() {
            SaveAllChanges();
        }

        void CheckOptionVersion() {
            if (!Prefs.Exists(optionVersionKey) || optionVersion != Prefs.GetInt(optionVersionKey)) {
                ClearRegistry();
            }
            Prefs.SetInt(optionVersionKey, optionVersion);
        }

        // A function to initializa the OptionSystem Object
        void Initialize() {
            CheckOptionVersion();
            // get all classes that have Options attributes
            var query = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where type.IsClass && type.GetCustomAttributes(typeof(OptionCategoryAttribute), true).Length > 0
                        select type;

            var allPropertiesStr = new StringBuilder();
            foreach (var type in query) {
                string categoryClassName = type.FullName;
                foreach (var property in type.GetProperties()) {
                    allPropertiesStr.Append(categoryClassName)
                        .Append(optionSeperator)
                        .Append(property.Name)
                        .Append(keySeperator);
                }
            }

            // Remove excess characters
            allPropertiesStr.Remove(allPropertiesStr.Length - 1, 1);
            string allProperties = allPropertiesStr.ToString();
            IEnumerable<string> currKeys = allProperties.Split(keySeperator);
            if (Prefs.Exists(allOptionsKey)) {
                IEnumerable<string> oldKeys = Prefs.GetString(allOptionsKey).Split(keySeperator);
                var obsoleteKeys = oldKeys.Except(currKeys);
                foreach (string key in obsoleteKeys) {
                    Prefs.Delete(key);
                }
            }

            Prefs.SetString(allOptionsKey, allProperties);
            Prefs.Save();

            var types =
                currKeys.Select(k => k.Split(optionSeperator)[0])
                    .Distinct()
                    .Select(t => Type.GetType(t))
                    .IgnoreNulls();
            foreach (Type type in types) {
                Get(type);
            }
        }

        // Function to case a generic object as its appropriate type
        public T Get<T>() {
            return (T)Get(typeof(T));
        }

        public object Get(Type type) {
            CategoryInfo category;
            if (!_categories.TryGetValue(type, out category)) {
                var obj = Activator.CreateInstance(type);
                category = new CategoryInfo(obj);
                _categories.Add(type, category);
            }
            return category.Instance;
        }

        // Function to save player option changes to memory
        public void SaveAllChanges() {
            foreach (OptionInfo option in Options) 
                option.Save();
            Log.Info("Options Saved");
        }

        public void RevertAllChanges() {
            foreach (OptionInfo option in Options)
                option.ResetValue();
            Log.Info("Options Reverted");
        }

        // Delete all option related entries in registry
        void ClearRegistry() {
            foreach (OptionInfo option in Options)
                option.Delete();
            _categories.Clear();
            Prefs.Delete(allOptionsKey);
        }
    }

    public sealed class CategoryInfo {

        public object Instance { get; private set; }
        public Type Type { get; private set; }
        public string Name { get; private set; }
        readonly List<OptionInfo> _options;

        public CategoryInfo(object instance) {
            _options = new List<OptionInfo>();
            Instance = instance;
            Type = Argument.NotNull(instance).GetType();
            var categoryAttribute = Type.GetCustomAttributes(true).OfType<OptionCategoryAttribute>().FirstOrDefault();
            if (categoryAttribute != null) {
                Name = categoryAttribute.Name ?? Type.Name;
            }

            foreach (var prop in Type.GetProperties()) {
                var attribute = prop.GetCustomAttributes(true).OfType<OptionAttribute>().FirstOrDefault();
                if (attribute != null) {
                    _options.Add(new OptionInfo(this, attribute, Type, prop));
                }
            }
        }

        public IEnumerable<OptionInfo> Options {
            get { return _options.Select(x => x); }
        }

    }

    public sealed class OptionInfo {

        public string Name { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public OptionAttribute Attribute { get; private set; }
        public CategoryInfo Category { get; private set; }
        public string Key { get; private set; }

        static readonly Dictionary<Type, IParser> _parser = new Dictionary<Type, IParser> {
            { typeof(string), new SimpleParser<string>(s => s) },
            { typeof(int), new SimpleParser<int>(int.Parse) },
            { typeof(float), new SimpleParser<float>(float.Parse) },
            { typeof(bool), new SimpleParser<bool>(bool.Parse) },
        };

        public OptionInfo(CategoryInfo category, OptionAttribute attr, Type type, PropertyInfo prop) {
            Name = attr.Name ?? prop.Name;
            Category = category;
            PropertyInfo = prop;
            Attribute = attr;
            Key = Argument.NotNull(type).FullName + OptionSystem.optionSeperator + 
                  Argument.NotNull(prop).Name;
            if (!Prefs.Exists(Key))
                Prefs.SetString(Key, prop.PropertyType.IsValueType ?
                    Activator.CreateInstance(prop.PropertyType).ToString() : 
                    "null");
            ResetValue();
        }

        public object GetPropertyValue() {
            return PropertyInfo.GetValue(Category.Instance, null);
        }

        public T GetPropertyValue<T>() {
            return (T)GetPropertyValue();
        }

        public void SetPropertyValue(object val) {
            PropertyInfo.SetValue(Category.Instance, val, null);
        }

        public object GetSavedValue() { return _parser[PropertyInfo.PropertyType].Parse(Prefs.GetString(Key)); }

        public void Save() {
            Prefs.SetString(Key, GetPropertyValue().ToString());
        }

        public void ResetValue() {
            SetPropertyValue(GetSavedValue());
        }

        internal void Delete() {
            Prefs.Delete(Key);
        }

    }
}
