using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace HouraiTeahouse.Options {

    public class OptionsManager : MonoBehaviour {

        [SerializeField]
        bool _autosave = true;

        readonly Dictionary<Type, CategoryInfo> _categories = new Dictionary<Type, CategoryInfo>();
            
        public IEnumerable<CategoryInfo> Categories {
            get { return _categories.Values; }
        }

        public IEnumerable<OptionInfo> Options {
            get { return _categories.Values.SelectMany(c => c.Options); }
        }

        public static bool Autosave { get; private set; }

        public static OptionsManager Instance { get; private set; }

        internal const char optionSeperator = ':';

        // OptionSystem's initialization must be done before the MetadataList call
        void Awake() {
            Autosave = _autosave;
            Instance = this;
        }

        void OnApplicationQuit() {
            SaveAllChanges();
        }

        public void LoadAllOptions(Assembly assembly = null) {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();
            // get all classes that have Options attributes
            var categories = from type in assembly.GetTypes()
                             where IsValidCategory(type)
                             select type;
            foreach(var category in categories)
                GetInfo(category);
        }

        internal static bool IsValidCategory(Type type) {
            return !type.IsAbstract && type.IsClass && 
                type.IsDefined(typeof(OptionCategoryAttribute), true);
        }

        // Function to case a generic object as its appropriate type
        public T Get<T>() {
            return (T)Get(typeof(T));
        }

        public object Get(Type type) {
            return GetInfo(type).Instance;
        }

        public CategoryInfo GetInfo<T>() {
            return GetInfo(typeof(T));
        }

        public CategoryInfo GetInfo(Type type)  {
            Argument.NotNull(type);
            if (!IsValidCategory(type))
                throw new ArgumentException("Category type must be valid!");
            CategoryInfo category;
            if (!_categories.TryGetValue(type, out category)) {
                var obj = Activator.CreateInstance(type);
                category = new CategoryInfo(obj);
                _categories.Add(type, category);
            }
            return category;
        }

        // Function to save player option changes to memory
        public void SaveAllChanges() {
            foreach (OptionInfo option in Options) 
                option.Save();
        }

        public void RevertAll(Assembly assembly = null) {
            LoadAllOptions(assembly);
            foreach (OptionInfo option in Options)
                option.Revert();
        }

        public void ResetAll(Assembly assembly = null) {
            PlayerPrefs.DeleteAll();
            LoadAllOptions(assembly);
            foreach (OptionInfo option in Options)
                option.Revert();
        }

    }
}
