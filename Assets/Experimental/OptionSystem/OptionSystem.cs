using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

namespace HouraiTeahouse
{
    public class OptionSystem : MonoBehaviour
    {
        Dictionary<Type, object> optionObjs = new Dictionary<Type, object>();
        List<CategoryInfo> metadataList;
        public List<CategoryInfo> MetadataList
        {
            get
            {
                if (metadataList == null)
                {
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
        void Awake()
        {
            Initialize();
        }

        // Debug call for saving changes,
        // just for testing purposes
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Log.Debug("Saved");
                SaveAllChanges();
            }
        }

        void CheckOptionVersion()
        {
            if (!PlayerPrefs.HasKey(optionVersionKey))
            {
                ClearRegistry();
            }
            else if (optionVersion != PlayerPrefs.GetInt(optionVersionKey))
            {
                ClearRegistry();
            }
            PlayerPrefs.SetInt(optionVersionKey, optionVersion);
        }

        // A function to initializa the OptionSystem Object
        void Initialize()
        {
            CheckOptionVersion();
            // get all classes that have Options attributes
            var query = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where type.IsClass && type.GetCustomAttributes(typeof(OptionCategory), true).Length > 0
                        select type;

            StringBuilder allPropertiesStr = new StringBuilder();
            foreach (var type in query)
            {
                string categoryClassName = type.FullName;
                string categoryAttrValue = string.Empty;
                var attrs = type.GetCustomAttributes(typeof(OptionCategory), true);
                foreach (var attr in attrs)
                {
                    if (attr is OptionCategory)
                    {
                        OptionCategory o = (OptionCategory)attr;
                        categoryAttrValue = o.Name;
                    }
                }

                foreach (var property in type.GetProperties())
                {
                    string propertyName = property.Name;
                    string propertyAttrValue = string.Empty;
                    foreach (var attr in attrs)
                    {
                        if (attr is Option)
                        {
                            Option o = (Option)attr;
                            propertyAttrValue = o.Name;
                        }
                    }
                    string propertyStr = categoryClassName + "*" + propertyName;

                    if (!PlayerPrefs.HasKey(propertyStr))
                    {
                        InitializePrefProperty(property.PropertyType, propertyStr);
                    }
                    allPropertiesStr.Append(propertyStr);
                    allPropertiesStr.Append(',');
                }
            }

            allPropertiesStr.Remove(allPropertiesStr.Length - 1, 1);
            if (PlayerPrefs.HasKey(allOptionsKey))
            {
                List<string> oldKeys = PlayerPrefs.GetString(allOptionsKey).Split(',').ToList();
                List<string> currKeys = allPropertiesStr.ToString().Split(',').ToList();
                var obsoleteKeys = oldKeys.Except(currKeys);
                foreach (var key in obsoleteKeys)
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }

            PlayerPrefs.SetString(allOptionsKey, allPropertiesStr.ToString());

            PlayerPrefs.Save();
        }

        void GenerateMetadata()
        {
            string keys = PlayerPrefs.GetString(allOptionsKey);
            string[] propFullNames = PlayerPrefs.GetString(allOptionsKey).Split(',');
            string lastTypeName = string.Empty;
            for (int i = 0; i < propFullNames.Length; i++)
            {
                string typeName = propFullNames[i].Split('*')[0];
                if (lastTypeName != typeName)
                {
                    Type type = Type.GetType(typeName);
                    metadataList.Add(new CategoryInfo(type, GetInstance(type)));
                }
                lastTypeName = typeName;
            }
        }

        // Function to case a generic object as its appropriate type
        public T Get<T>()
        {
            object obj;
            Type type = typeof(T);
            if (optionObjs.ContainsKey(type))
            {
                optionObjs.TryGetValue(type, out obj);
            }
            else
            {
                obj = Activator.CreateInstance(type);

                foreach (var prop in type.GetProperties())
                {
                    string key = type.ToString() + '*' + prop.Name;
                    object propertyValue = GetValueFromPrefs(key);
                    prop.SetValue(obj, propertyValue, null);
                }

                optionObjs.Add(type, obj);
            }
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public object GetInstance(Type type)
        {
            object obj;
            if (optionObjs.ContainsKey(type))
            {
                optionObjs.TryGetValue(type, out obj);
            }
            else
            {
                obj = Activator.CreateInstance(type);

                foreach (var prop in type.GetProperties())
                {
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
            foreach (var pair in optionObjs)
            {
                Type type = pair.Key;
                string typeName = type.FullName;

                foreach (var property in type.GetProperties())
                {
                    string propName = property.Name;
                    string keyStr = typeName + '*' + propName;
                    string valStr = property.GetValue(pair.Value, null).ToString();

                    PlayerPrefs.SetString(keyStr, valStr);
                }
            }
        }

        // Delete all option related entries in registry
        void ClearRegistry()
        {
            string allKeys = PlayerPrefs.GetString(allOptionsKey);
            string[] allMembers = allKeys.Split(',');
            for (int i = 0; i < allMembers.Length; i++)
            {
                PlayerPrefs.DeleteKey(allMembers[i]);
            }
            PlayerPrefs.DeleteKey(allOptionsKey);
        }
        // Get player option data from the registry
        void GetDataFromPrefs()
        {
            string allKeys = PlayerPrefs.GetString(allOptionsKey);
            string[] allMembers = allKeys.Split(',');
            string previousClassName = string.Empty;
            object obj = null;
            for (int i = 0; i < allMembers.Length; i++)
            {
                string[] strs = allMembers[i].Split('*');
                string className = strs[0];
                string propertyName = strs[1];
                object propertyValue = GetValueFromPrefs(allMembers[i]);

                Type type = Type.GetType(className);
                if (className != previousClassName)
                {
                    obj = Activator.CreateInstance(type);
                    optionObjs.Add(type, obj);
                }

                type.GetProperty(propertyName).SetValue(obj, propertyValue, null);

                previousClassName = className;
            }
        }
        // Object to hold player option values
        object GetValueFromPrefs(string key)
        {
            string[] keyStrs = key.Split('*');
            string className = keyStrs[0];
            string propName = keyStrs[1];
            string typeName = Type.GetType(className).GetProperty(propName).PropertyType.ToString();
            string valStr = PlayerPrefs.GetString(key);
            Type type = Type.GetType(typeName);
            object val = null;
            if (type == typeof(int))
            {
                val = int.Parse(valStr);
            }
            else if (type == typeof(float))
            {
                val = float.Parse(valStr);
            }
            else if (type == typeof(bool))
            {
                val = bool.Parse(valStr);
            }
            else if (type == typeof(string))
            {
                val = valStr;
            }
            else
            {
                Debug.LogError(key + " has an unsupported type");

            }
            return val;
        }
        // Function to initialize a new option as a given type of option
        void InitializePrefProperty(System.Type type, string key)
        {
            if (type == typeof(float))
            {
                PlayerPrefs.SetString(key, "0.0");
            }
            else if (type == typeof(int))
            {
                PlayerPrefs.SetString(key, "0");
            }
            else if (type == typeof(string))
            {
                PlayerPrefs.SetString(key, string.Empty);
            }
            else if (type == typeof(bool))
            {
                PlayerPrefs.SetString(key, "false");
            }
            else
            {
                Debug.LogError(key + " has an unsupported property type " + type.ToString());
            }
        }

        public class CategoryInfo
        {
            Type type;
            string categoryName;
            List<OptionInfo> optionList;

            public CategoryInfo(Type type, object instance)
            {
                optionList = new List<OptionInfo>();
                this.type = type;
                foreach (var attr in type.GetCustomAttributes(true))
                {
                    Type attrType = attr.GetType();
                    if (attrType.IsAssignableFrom(typeof(OptionCategory)) || attrType.IsSubclassOf(typeof(OptionCategory)))
                    {
                        categoryName = ((OptionCategory)attr).Name;
                    }
                }

                foreach (var prop in type.GetProperties())
                {
                    foreach (var attr in prop.GetCustomAttributes(true))
                    {
                        if (attr.GetType().IsSubclassOf(typeof(Option)))
                        {
                            optionList.Add(new OptionInfo((Option)attr, prop, instance));
                        }
                    }
                }
            }

            public string ClassName
            {
                get
                {
                    return type.FullName;
                }
            }

            public Type ClassType
            {
                get
                {
                    return type;
                }
            }

            public string CategoryName
            {
                get
                {
                    return categoryName;
                }
            }

            public List<OptionInfo> OptionList
            {
                get
                {
                    return optionList;
                }
            }

        }

        public class OptionInfo
        {
            object instance;
            PropertyInfo propertyInfo;
            Option optionAttr;

            public OptionInfo(Option attr, PropertyInfo prop, object instance)
            {
                this.instance = instance;
                propertyInfo = prop;
                optionAttr = attr;
            }

            public object GetPropertyValue()
            {
                return propertyInfo.GetValue(instance, null);
            }

            public void SetPropertyValue(object val)
            {
                propertyInfo.SetValue(instance, val, null);
            }

            public string ProperptyName
            {
                get
                {
                    return propertyInfo.Name;
                }
            }

            public string OptionName
            {
                get
                {
                    return optionAttr.Name;
                }
            }

            public PropertyInfo PropertyInfo
            {
                get
                {
                    return propertyInfo;
                }
            }

            public Option OptionAttr
            {
                get
                {
                    return optionAttr;
                }
            }
        }
    }
}