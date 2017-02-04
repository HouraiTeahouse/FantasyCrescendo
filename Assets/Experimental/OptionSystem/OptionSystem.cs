using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

namespace HouraiTeahouse {

    public interface IParser {
        object Parse(string obj);
    }

    public class SimpleParser<T> : IParser {
        readonly Func<string, T> _parseFunc;
        public SimpleParser(Func<string, T> parseFun) { _parseFunc = parseFun; }
        public object Parse(string obj) { return _parseFunc(obj); }
    }

    public class OptionSystem : MonoBehaviour {
        readonly Dictionary<Type, object> _optionMap = new Dictionary<Type, object>();
        readonly List<CategoryInfo> _metadataList = new List<CategoryInfo>();
        readonly Dictionary<Type, IParser> _parser = new Dictionary<Type, IParser> {
            { typeof(string), new SimpleParser<string>(s => s) },
            { typeof(int), new SimpleParser<int>(int.Parse) },
            { typeof(float), new SimpleParser<float>(float.Parse) },
            { typeof(bool), new SimpleParser<bool>(bool.Parse) },
        };
            
        public IEnumerable<CategoryInfo> Categories {
            get { return _metadataList.Select(x => x); }
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
        const char optionSeperator = '*';
        const string optionVersionKey = "OptionVersion";

        // OptionSystem's initialization must be done before the MetadataList call
        void Awake() {
            Initialize();
        }

#if UNITY_EDITOR
        // Debug call for saving changes,
        // just for testing purposes
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S)) {
                SaveAllChanges();
            }
        }
#endif

        void CheckOptionVersion() {
            if (!Prefs.Exists(optionVersionKey)) {
                ClearRegistry();
            } else if (optionVersion != Prefs.GetInt(optionVersionKey)) {
                ClearRegistry();
            }
            Prefs.SetInt(optionVersionKey, optionVersion);
        }

        // A function to initializa the OptionSystem Object
        void Initialize() {
            CheckOptionVersion();
            // get all classes that have Options attributes
            var query = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where type.IsClass && type.GetCustomAttributes(typeof(OptionCategory), true).Length > 0
                        select type;

            var allPropertiesStr = new StringBuilder();
            foreach (var type in query) {
                string categoryClassName = type.FullName;
                foreach (var property in type.GetProperties()) {
                    string propertyStr = categoryClassName + optionSeperator + property.Name;
                    if (!Prefs.Exists(propertyStr)) {
                        InitializePrefProperty(property.PropertyType, propertyStr);
                    }
                    allPropertiesStr.Append(propertyStr).Append(keySeperator);
                }
            }

            allPropertiesStr.Remove(allPropertiesStr.Length - 1, 1);
            IEnumerable<string> currKeys = allPropertiesStr.ToString().Split(keySeperator);
            if (Prefs.Exists(allOptionsKey)) {
                IEnumerable<string> oldKeys = Prefs.GetString(allOptionsKey).Split(keySeperator);
                var obsoleteKeys = oldKeys.Except(currKeys);
                foreach (string key in obsoleteKeys) {
                    Prefs.Delete(key);
                }
            }

            Prefs.SetString(allOptionsKey, allPropertiesStr.ToString());
            Prefs.Save();

            foreach (string fullname in currKeys) {
                string typeName = fullname.Split(optionSeperator)[0];
                Type type = Type.GetType(typeName);
                if (type != null && !_optionMap.ContainsKey(type)) {
                    _metadataList.Add(new CategoryInfo(type, Get(type)));
                }
            }
        }

        // Function to case a generic object as its appropriate type
        public T Get<T>() {
            return (T)Get(typeof(T));
        }

        public object Get(Type type) {
            object obj;
            if (!_optionMap.TryGetValue(type, out obj)) {
                obj = Activator.CreateInstance(type);

                foreach (var prop in type.GetProperties()) {
                    string key = type.ToString() + optionSeperator + prop.Name;
                    object propertyValue = GetValueFromPrefs(key);
                    prop.SetValue(obj, propertyValue, null);
                }

                _optionMap.Add(type, obj);
            }
            return obj;
        }

        // Function to save player option changes to memory
        public void SaveAllChanges() {
            foreach (var pair in _optionMap) {
                Type type = pair.Key;
                string typeName = type.FullName;

                foreach (var property in type.GetProperties()) {
                    string propName = property.Name;
                    string keyStr = typeName + optionSeperator + propName;
                    string valStr = property.GetValue(pair.Value, null).ToString();

                    Prefs.SetString(keyStr, valStr);
                }
            }
            Log.Info("Options Saved");
        }

        public void RevertAllChanges() {
            foreach (Type type in _optionMap.Keys) {
                object instance = _optionMap[type];
                foreach (PropertyInfo prop in type.GetProperties()) {
                    string key = type.FullName + '*' + prop.Name;
                    prop.SetValue(instance, GetValueFromPrefs(key), null);
                }
            }
            Log.Info("Options Reverted");
        }

        // Delete all option related entries in registry
        void ClearRegistry() {
            string allKeys = Prefs.GetString(allOptionsKey);
            string[] allMembers = allKeys.Split(keySeperator);
            foreach (string member in allMembers) {
                Prefs.Delete(member);
            }
            Prefs.Delete(allOptionsKey);
        }

        // Get player option data from the registry
        void GetDataFromPrefs() {
            string allKeys = Prefs.GetString(allOptionsKey);
            string[] allMembers = allKeys.Split(keySeperator);
            string previousClassName = string.Empty;
            object obj = null;
            foreach (string member in allMembers) {
                string[] strs = member.Split(optionSeperator);
                string className = strs[0];
                string propertyName = strs[1];
                object propertyValue = GetValueFromPrefs(member);

                Type type = Type.GetType(className);
                if (className != previousClassName) {
                    obj = Activator.CreateInstance(type);
                    _optionMap.Add(type, obj);
                }

                type.GetProperty(propertyName).SetValue(obj, propertyValue, null);

                previousClassName = className;
            }
        }

        // Object to hold player option values
        object GetValueFromPrefs(string key) {
            string[] keyStrs = key.Split(optionSeperator);
            string className = keyStrs[0];
            string propName = keyStrs[1];
            string typeName = Type.GetType(className).GetProperty(propName).PropertyType.ToString();
            string valStr = Prefs.GetString(key);
            Type type = Type.GetType(typeName);
            object val = null;
            IParser parser;
            if (type != null && _parser.TryGetValue(type, out parser)) {
                val = parser.Parse(valStr);
            } else {
                Log.Error(key + " has an unsupported type");
            }
            return val;
        }
        // Function to initialize a new option as a given type of option
        void InitializePrefProperty(Type type, string key) {
            if (type == typeof(float)) {
                Prefs.SetString(key, "0.0");
            } else if (type == typeof(int)) {
                Prefs.SetString(key, "0");
            } else if (type == typeof(string)) {
                Prefs.SetString(key, string.Empty);
            } else if (type == typeof(bool)) {
                Prefs.SetString(key, "false");
            } else {
                Log.Error("{0} has an unsupported property type {1}", key, type);
            }
        }
    }

    public class CategoryInfo {

        public Type Type { get; private set; }
        public string Name { get; private set; }
        readonly List<OptionInfo> _options;

        public CategoryInfo(Type type, object instance) {
            _options = new List<OptionInfo>();
            Type = type;
            var categoryAttribute = type.GetCustomAttributes(true).OfType<OptionCategory>().FirstOrDefault();
            if (categoryAttribute != null) {
                Name = categoryAttribute.Name ?? type.Name;
            }

            foreach (var prop in type.GetProperties()) {
                var attribute = prop.GetCustomAttributes(true).OfType<Option>().FirstOrDefault();
                if (attribute != null) {
                    _options.Add(new OptionInfo(attribute, prop, instance) {Category = this});
                }
            }
        }

        public IEnumerable<OptionInfo> Options {
            get { return _options.Select(x => x); }
        }

    }

    public class OptionInfo {

        readonly object _instance;
        public PropertyInfo PropertyInfo { get; private set; }
        public Option Attribute { get; private set; }
        public CategoryInfo Category { get; internal set; }

        public OptionInfo(Option attr, PropertyInfo prop, object instance) {
            _instance = instance;
            PropertyInfo = prop;
            Attribute = attr;
        }

        public object GetPropertyValue() {
            return PropertyInfo.GetValue(_instance, null);
        }

        public void SetPropertyValue(object val) {
            PropertyInfo.SetValue(_instance, val, null);
        }

    }
}
