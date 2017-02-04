using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

namespace HouraiTeahouse
{
    public class OptionSystem : MonoBehaviour {
        readonly Dictionary<Type, object> optionObjs = new Dictionary<Type, object>();
        List<CategoryInfo> metadataList;
        public List<CategoryInfo> MetadataList {
            get {
                if (metadataList == null) {
                    metadataList = new List<CategoryInfo>();
                    GenerateMetadata();
                }
                return metadataList;
            }
        }

        // in registry there will be an entry under this key,
        // which includes all options' key.
        // This exists solely because Unity does not have API
        // to traverse through all PlayerPrefs.
        // This is used for removing obsolete options.

        string allOptionsKey = "OptionNames";

        // A version tracker for deleting old registries
        // only updates when serialized data format becomes different
        // for example, when PlayerPrefs value changed from "type,value" to "value" only
        const int optionVersion = 1;
        string optionVersionKey = "OptionVersion";

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
                    string propertyName = property.Name;
                    string propertyStr = categoryClassName + "*" + propertyName;

                    if (!Prefs.Exists(propertyStr)) {
                        InitializePrefProperty(property.PropertyType, propertyStr);
                    }
                    allPropertiesStr.Append(propertyStr);
                    allPropertiesStr.Append(',');
                }
            }

            allPropertiesStr.Remove(allPropertiesStr.Length - 1, 1);
            if (Prefs.Exists(allOptionsKey)) {
                List<string> oldKeys = Prefs.GetString(allOptionsKey).Split(',').ToList();
                List<string> currKeys = allPropertiesStr.ToString().Split(',').ToList();
                var obsoleteKeys = oldKeys.Except(currKeys);
                foreach (var key in obsoleteKeys) {
                    Prefs.Exists(key);
                }
            }

            Prefs.SetString(allOptionsKey, allPropertiesStr.ToString());
            Prefs.Save();
        }

        void GenerateMetadata() {
            string[] propFullNames = Prefs.GetString(allOptionsKey).Split(',');
            string lastTypeName = string.Empty;
            foreach (string fullname in propFullNames) {
                string typeName = fullname.Split('*')[0];
                if (lastTypeName != typeName) {
                    Type type = Type.GetType(typeName);
                    metadataList.Add(new CategoryInfo(type, Get(type)));
                }
                lastTypeName = typeName;
            }
        }

        // Function to case a generic object as its appropriate type
        public T Get<T>() {
            return (T)Get(typeof(T));
        }

        public object Get(Type type) {
            object obj;
            if (!optionObjs.TryGetValue(type, out obj)) {
                obj = Activator.CreateInstance(type);

                foreach (var prop in type.GetProperties()) {
                    string key = type.ToString() + '*' + prop.Name;
                    object propertyValue = GetValueFromPrefs(key);
                    prop.SetValue(obj, propertyValue, null);
                }

                optionObjs.Add(type, obj);
            }
            return obj;
        }

        // Function to save player option changes to memory
        public void SaveAllChanges()
        {
            foreach (var pair in optionObjs) {
                Type type = pair.Key;
                string typeName = type.FullName;

                foreach (var property in type.GetProperties()) {
                    string propName = property.Name;
                    string keyStr = typeName + '*' + propName;
                    string valStr = property.GetValue(pair.Value, null).ToString();

                    Prefs.SetString(keyStr, valStr);
                }
            }

            Log.Debug("Options Saved");
        }

        public void RevertAllChanges()
        {
            string[] typeNames = new string[optionObjs.Count];
            int i = 0;
            foreach (var pair in optionObjs)
            {
                Type type = pair.Key;
                string typeName = type.FullName;
                typeNames[i] = typeName;
                i++;
            }

            foreach (string name in typeNames) {
                Type type = Type.GetType(name);
                foreach (var prop in type.GetProperties())
                {
                    string key = type.FullName + '*' + prop.Name;
                    object instance;
                    optionObjs.TryGetValue(type, out instance);
                    prop.SetValue(instance, GetValueFromPrefs(key), null);
                }
            }

            Log.Debug("Options Reverted");
        }

        // Delete all option related entries in registry
        void ClearRegistry() {
            string allKeys = Prefs.GetString(allOptionsKey);
            string[] allMembers = allKeys.Split(',');
            foreach (string member in allMembers) {
                Prefs.Delete(member);
            }
            Prefs.Delete(allOptionsKey);
        }

        // Get player option data from the registry
        void GetDataFromPrefs()
        {
            string allKeys = Prefs.GetString(allOptionsKey);
            string[] allMembers = allKeys.Split(',');
            string previousClassName = string.Empty;
            object obj = null;
            foreach (string member in allMembers) {
                string[] strs = member.Split('*');
                string className = strs[0];
                string propertyName = strs[1];
                object propertyValue = GetValueFromPrefs(member);

                Type type = Type.GetType(className);
                if (className != previousClassName) {
                    obj = Activator.CreateInstance(type);
                    optionObjs.Add(type, obj);
                }

                type.GetProperty(propertyName).SetValue(obj, propertyValue, null);

                previousClassName = className;
            }
        }

        // Object to hold player option values
        object GetValueFromPrefs(string key) {
            string[] keyStrs = key.Split('*');
            string className = keyStrs[0];
            string propName = keyStrs[1];
            string typeName = Type.GetType(className).GetProperty(propName).PropertyType.ToString();
            string valStr = Prefs.GetString(key);
            Type type = Type.GetType(typeName);
            object val = null;
            if (type == typeof(int)) {
                val = int.Parse(valStr);
            } else if (type == typeof(float)) {
                val = float.Parse(valStr);
            } else if (type == typeof(bool)) {
                val = bool.Parse(valStr);
            } else if (type == typeof(string)) {
                val = valStr;
            } else {
                Debug.LogError(key + " has an unsupported type");
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
                Debug.LogError(key + " has an unsupported property type " + type.ToString());
            }
        }

        public class CategoryInfo {

            readonly Type _type;
            readonly string _categoryName;
            readonly List<OptionInfo> _optionList;

            public CategoryInfo(Type type, object instance) {
                _optionList = new List<OptionInfo>();
                _type = type;
                foreach (var attr in type.GetCustomAttributes(true)) {
                    Type attrType = attr.GetType();
                    if (attrType.IsAssignableFrom(typeof(OptionCategory)) || attrType.IsSubclassOf(typeof(OptionCategory))) {
                        _categoryName = ((OptionCategory)attr).Name;
                    }
                }

                foreach (var prop in type.GetProperties()) {
                    foreach (var attr in prop.GetCustomAttributes(true)) {
                        if (attr.GetType().IsSubclassOf(typeof(Option))) {
                            _optionList.Add(new OptionInfo((Option)attr, prop, instance));
                        }
                    }
                }
            }

            public string ClassName {
                get { return _type.FullName; }
            }

            public Type ClassType {
                get { return _type; }
            }

            public string CategoryName {
                get { return _categoryName; }
            }

            public List<OptionInfo> OptionList {
                get { return _optionList; }
            }

        }

        public class OptionInfo {

            readonly object _instance;
            readonly PropertyInfo _propertyInfo;
            readonly Option _optionAttr;

            public OptionInfo(Option attr, PropertyInfo prop, object instance) {
                _instance = instance;
                _propertyInfo = prop;
                _optionAttr = attr;
            }

            public object GetPropertyValue() {
                return _propertyInfo.GetValue(_instance, null);
            }

            public void SetPropertyValue(object val) {
                _propertyInfo.SetValue(_instance, val, null);
            }

            public string PropertyName {
                get { return _propertyInfo.Name; }
            }

            public string OptionName {
                get { return _optionAttr.Name; }
            }

            public PropertyInfo PropertyInfo {
                get { return _propertyInfo; }
            }

            public Option OptionAttr {
                get { return _optionAttr; }
            }
        }
    }
}
