using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HouraiTeahouse.Options {

    /// <summary>
    /// Metadata object for a single option.
    /// See <see cref="HouraiTeahouse.Options.OptionsManager">.
    /// </summary>
    public sealed class OptionInfo {

        /// <summary>
        /// Gets the human readable name of the option.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the metadata descriptor for the original property for the option.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets the defining OptionAttribute for the option.
        /// </summary>
        public OptionAttribute Attribute { get; private set; }

        /// <summary>
        /// Gets the category the option belongs to.
        /// </summary>
        public CategoryInfo Category { get; private set; }

        /// <summary>
        /// Gets the PlayerPrefs key used to store the value.
        /// Note: all values are stored as strings in PlayerPrefs.
        /// </summary>
        public string Key { get; private set; }

        Delegate _listeners;
        const char optionSeperator = ':';

        static readonly Dictionary<Type, Func<string, object>> _parser = new Dictionary<Type, Func<string, object>> {
            { typeof(string), s => s },
            { typeof(int), s => int.Parse(s) },
            { typeof(float), s => float.Parse(s) },
            { typeof(bool), s => bool.Parse(s) },
        };

        internal OptionInfo(CategoryInfo category, OptionAttribute attr, Type type, PropertyInfo prop) {
            Name = attr.Name ?? prop.Name;
            Category = category;
            PropertyInfo = prop;
            Attribute = attr;
            Key = Argument.NotNull(type).FullName + optionSeperator + 
                  Argument.NotNull(prop).Name;
            var propType = prop.PropertyType;
            if (!Prefs.Exists(Key))
                Reset();
            Revert();
        }

        object CreateDefaultValue(Type type) {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            else
                return "null";
        }

        /// <summary>
        /// Gets the options current value. This will change as the 
        /// object's property is changed. 
        /// </summary>
        /// <returns>the property value</returns>
        public object GetPropertyValue() {
            return PropertyInfo.GetValue(Category.Instance, null);
        }

        /// <summary>
        /// Gets the options current value. This will change as the 
        /// object's property is changed. 
        /// </summary>
        /// <typeparam name="T">the type of value to try to get</typeparam>
        /// <exception cref="System.InvalidCastException">
        ///     the stored value is not of type <typeparamref name="T">
        /// </exception>
        /// <returns>the property value</returns>
        public T GetPropertyValue<T>() {
            return (T)GetPropertyValue();
        }

        /// <summary>
        /// Sets the options current value.
        /// If <see cref="OptionManager.Autosave"> is true, this will automatically
        /// save the value to PlayerPrefs.
        /// </summary>
        /// <param name="val">the value to assign to the option.</param>
        public void SetPropertyValue(object val) {
            var oldValue = GetPropertyValue();
            PropertyInfo.SetValue(Category.Instance, val, null);
            if (oldValue != val && _listeners != null) 
                _listeners.DynamicInvoke(oldValue, val);
            if (OptionsManager.Autosave)
                Save();
        }

        /// <summary>
        /// Gets the saved value from PlayerPrefs.
        /// </summary>
        /// <returns></returns>
        public object GetSavedValue() { 
            return _parser[PropertyInfo.PropertyType](Prefs.GetString(Key)); 
        }

        /// <summary>
        /// Saves the current option value to PlayerPrefs.
        /// </summary>
        public void Save() {
            Prefs.SetString(Key, GetPropertyValue().ToString());
        }

        /// <summary>
        /// Revert the currently loaded value to the one stored in PlayerPrefs.
        /// </summary>
        public void Revert() {
            SetPropertyValue(GetSavedValue());
        }

        /// <summary>
        /// Resets the current value and saved value to the option's default value.
        /// </summary>
        public void Reset() {
            var propType = PropertyInfo.PropertyType;
            var val = Attribute.DefaultValue;
            if (val == null) {
                val = CreateDefaultValue(propType);
            } else if (val.GetType() != propType) {
                Log.Warning("Default value for {0} option does not match target type.", PropertyInfo);
                val = CreateDefaultValue(propType);
            }
            SetPropertyValue(val);
            Prefs.SetString(Key, val.ToString());
        }

        /// <summary>
        /// Adds a change listener. <paramref name="callback"> will be executed every time each
        /// time the value of the option changes.
        /// </summary>
        /// <typeparam name="T">the expected type of the value of the option.</typeparam>
        /// <exception cref="System.ArgumentException">
        ///     <typeparamref name="T"> is not compatible with the type of the option
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="callback"> is null.</exception>
        /// <param name="callback">the callback for the listener</param>
        public void AddListener<T>(Action<T, T> callback) {
            Argument.NotNull(callback);
            if (!PropertyInfo.PropertyType.IsAssignableFrom(typeof(T)))
                throw new ArgumentException("Cannot add listener for a type that does not match ");
            _listeners = Delegate.Combine(_listeners, callback);
        }

        /// <summary>
        /// Removes a change listener from the option.
        /// </summary>
        /// <param name="callback">the callback for the listener</param>
        public void RemoveListener<T>(Action<T, T> callback) {
            Argument.NotNull(callback);
            _listeners = Delegate.RemoveAll(_listeners, callback);
        }

    }

}