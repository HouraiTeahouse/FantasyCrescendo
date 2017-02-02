using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

public class OptionSystem : MonoBehaviour
{
    Dictionary<Type, object> optionObjs = new Dictionary<Type, object>();

    // in registry there will be an entry under this key,
    // which includes all options' key.
    // This exists solely because Unity does not have API
    // to traverse through all PlayerPrefs.
    // This is used for removing obsolete options.

    string allOptionsKey = "OptionNames";

    void Start()
    {
        Initialize();
        GetDataFromPrefs();
        var audio = Get<AudioOptions>();
        audio.Print();
        audio.master = 1.0f;
        audio.bgm = 2.0f;
        audio.sfx = 3.0f;
        audio.Print();
        var some = Get<SomeOptions>();
        some.Print();
        some.someBool = true;
        some.someFloat = 123.45f;
        some.someInt = 21341;
        some.someString = "afiohqrwoiy";
        some.Print();

        SaveAllChanges();

        optionObjs.Clear();
        GetDataFromPrefs();
        audio = Get<AudioOptions>();
        audio.Print();
        some = Get<SomeOptions>();
        some.Print();
    }

	void Initialize()
    {
        // get all classes that have Options attributes
        var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.IsClass && t.GetCustomAttributes(typeof(OptionCategory), true).Length > 0
                select t;

        StringBuilder allPropertiesStr = new StringBuilder();
        foreach (var t in q)
        {

            string categoryClassName = t.FullName;
            string categoryAttrValue = string.Empty;
            var attrs = t.GetCustomAttributes(typeof(OptionCategory), true);
            foreach (var attr in attrs)
            {
                if (attr is OptionCategory)
                {
                    OptionCategory o = (OptionCategory)attr;
                    categoryAttrValue = o.Name;
                }
            }

            foreach (var p in t.GetProperties())
            {
                string propertyName = p.Name;
                string propertyAttrValue = string.Empty;
                foreach (var attr in attrs)
                {
                    if (attr is Option)
                    {
                        Option o = (Option)attr;
                        propertyAttrValue = o.Name;
                    }
                }
                StringBuilder propertyStr = new StringBuilder();
                propertyStr.Append(categoryClassName);
                propertyStr.Append('*');
                propertyStr.Append(propertyName);
                if (!PlayerPrefs.HasKey(propertyStr.ToString()))
                {
                    InitializePrefProperty(p.PropertyType, propertyStr.ToString());
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

    T Get<T>()
    {
        object obj = optionObjs[typeof(T)];
        return (T)Convert.ChangeType(obj, typeof(T));
    }

    void SaveAllChanges()
    {
        foreach(var pair in optionObjs)
        {
            Type t = pair.Key;
            string typeName = t.FullName;
            
            foreach (var p in t.GetProperties())
            {
                StringBuilder keyStr = new StringBuilder();
                string propName = p.Name;
                keyStr.Append(typeName);
                keyStr.Append('*');
                keyStr.Append(propName);

                StringBuilder valStr = new StringBuilder();
                valStr.Append(p.PropertyType.FullName);
                valStr.Append(',');
                valStr.Append(p.GetValue(pair.Value, null));

                PlayerPrefs.SetString(keyStr.ToString(), valStr.ToString());
            }
        }
    }

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
            
            Type t = Type.GetType(className);
            if (className != previousClassName)
            {
                obj = Activator.CreateInstance(t);
                optionObjs.Add(t, obj);
            }

            t.GetProperty(propertyName).SetValue(obj, propertyValue, null);
            
            previousClassName = className;
        }
    }

    object GetValueFromPrefs(string key)
    {
        string raw = PlayerPrefs.GetString(key);
        string[] strs = raw.Split(',');
        string typeName = strs[0];
        string valStr = strs[1];
        Type t = Type.GetType(typeName);
        object val = null;
        if (t == typeof(int))
        {
            val = int.Parse(valStr);
        }
        else if (t == typeof(float))
        {
            val = float.Parse(valStr);
        }
        else if (t == typeof(bool))
        {
            val = bool.Parse(valStr);
        }
        else if (t == typeof(string))
        {
            val = valStr;
        }
        else
        {
            Debug.LogError(key + " has an unsupported type");
        }
        return val;
    }

    void InitializePrefProperty(System.Type type, string key)
    {
        if (type == typeof(float))
        {
            PlayerPrefs.SetString(key, type.FullName + ",0.0");
        }
        else if (type == typeof(int))
        {
            PlayerPrefs.SetString(key, type.FullName + ",0");
        }
        else if (type == typeof(string))
        {
            PlayerPrefs.SetString(key, type.FullName + ",");
        }
        else if (type == typeof(bool))
        {
            PlayerPrefs.SetString(key, type.FullName + ",false");
        }
        else
        {
            Debug.LogError(key + " has an unsupported property type " + type.ToString());
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
