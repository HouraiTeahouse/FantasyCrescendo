using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HouraiTeahouse.Options {

    public sealed class OptionInfo {

        public string Name { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public OptionAttribute Attribute { get; private set; }
        public CategoryInfo Category { get; private set; }
        public string Key { get; private set; }
        Delegate _listeners;

        static readonly Dictionary<Type, Func<string, object>> _parser = new Dictionary<Type, Func<string, object>> {
            { typeof(string), s => s },
            { typeof(int), s => int.Parse(s) },
            { typeof(float), s => float.Parse(s) },
            { typeof(bool), s => bool.Parse(s) },
        };

        public void AddListener<T>(Action<T, T> callback) {
            Argument.NotNull(callback);
            if (!PropertyInfo.PropertyType.IsAssignableFrom(typeof(T)))
                throw new ArgumentException("Cannot add listener for a type that does not match ");
            _listeners = Delegate.Combine(_listeners, callback);
        }

        public void RemoveListener<T>(Action<T, T> callback) {
            Argument.NotNull(callback);
            _listeners = Delegate.RemoveAll(_listeners, callback);
        }

        internal OptionInfo(CategoryInfo category, OptionAttribute attr, Type type, PropertyInfo prop) {
            Name = attr.Name ?? prop.Name;
            Category = category;
            PropertyInfo = prop;
            Attribute = attr;
            Key = Argument.NotNull(type).FullName + OptionsManager.optionSeperator + 
                  Argument.NotNull(prop).Name;
            var propType = prop.PropertyType;
            if (!Prefs.Exists(Key)) {
                var val = attr.DefaultValue;
                if (val == null) {
                    val = CreateDefaultValue(propType);
                } else if (val.GetType() != propType) {
                    Log.Warning("Default value for {0} option does not match target type.", prop);
                    val = CreateDefaultValue(propType);
                }
                Prefs.SetString(Key, val.ToString());
            }
            ResetValue();
        }

        object CreateDefaultValue(Type type) {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            else
                return "null";
        }

        public object GetPropertyValue() {
            return PropertyInfo.GetValue(Category.Instance, null);
        }

        public T GetPropertyValue<T>() {
            return (T)GetPropertyValue();
        }

        public void SetPropertyValue(object val) {
            var oldValue = GetPropertyValue();
            PropertyInfo.SetValue(Category.Instance, val, null);
            if (oldValue != val && _listeners != null) 
                _listeners.DynamicInvoke(oldValue, val);
            if (OptionsManager.Autosave)
                Save();
        }

        public object GetSavedValue() { return _parser[PropertyInfo.PropertyType](Prefs.GetString(Key)); }

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