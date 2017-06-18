using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace HouraiTeahouse.Options {

    /// <summary>
    /// The main manager of the HouraiOptions option system.
    /// </summary>
    [HelpURL("https://github.com/HouraiTeahouse/HouraiOptions")]
    public class OptionsManager : MonoBehaviour {

        [SerializeField]
        bool _autosave = true;

        readonly Dictionary<Type, CategoryInfo> _categories = new Dictionary<Type, CategoryInfo>();
            
        /// <summary>
        /// Gets an enumeration of all currently loaded option categories.
        /// </summary>
        public IEnumerable<CategoryInfo> Categories {
            get { return _categories.Values; }
        }

        /// <summary>
        /// Gets a enumeration of all currently loaded options.
        /// </summary>
        public IEnumerable<OptionInfo> Options {
            get { return _categories.Values.SelectMany(c => c.Options); }
        }

        /// <summary>
        /// Whether or not options automatically save their changes when set or not.
        /// If true, an alteration to PlayerPrefs will be done each time an option is written to.
        /// </summary>
        /// <returns></returns>
        public static bool Autosave { get; private set; }

        /// <summary>
        /// The singleton instance of the OptionsManager.
        /// </summary>
        public static OptionsManager Instance { get; private set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Autosave = _autosave;
            Instance = this;
        }

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        void OnApplicationQuit() {
            SaveAllChanges();
        }

        /// <summary>
        /// Loads all options from <paramref name="assembly">.
        /// If <paramref name="assembly"> is null, the current executing assembly
        /// is used.
        /// </summary>
        /// <param name="assembly">the assembly to load option values from</param>
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

        /// <summary>
        /// Checks if a type is a valid category type.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <returns>true, if <paramref name="type"> is valid, false otherwise.</returns>
        internal static bool IsValidCategory(Type type) {
            return !type.IsAbstract && type.IsClass && 
                type.IsDefined(typeof(OptionCategoryAttribute), true);
        }

        /// <summary>
        /// Gets the option category object instance for a category.
        /// </summary>
        /// <typeparam name="T">The type of the category to get</typeparam>
        /// <exception cref="System.ArgumentException"><typeparamref name="T"> is an invalid type.</exception>
        /// <returns>The object instance for the category</returns>
        public T Get<T>() {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Gets the option category object instance for a category.
        /// </summary>
        /// <typeparam name="type">The type of the category to get</typeparam>
        /// <exception cref="System.ArgumentException"><paramref name="type"> is an invalid type.</exception>
        /// <exception cref="System.ArgumentNullException"><typeparamref name="type"> is null.</exception>
        /// <returns>The object instance for the category</returns>
        public object Get(Type type) {
            return GetInfo(type).Instance;
        }

        /// <summary>
        /// Gets the option category metadata object for a category.
        /// </summary>
        /// <typeparam name="T">The type of the category to get</typeparam>
        /// <exception cref="System.ArgumentException"><typeparamref name="T"> is an invalid type.</exception>
        /// <returns>The metadata instance for the category</returns>
        public CategoryInfo GetInfo<T>() {
            return GetInfo(typeof(T));
        }

        /// <summary>
        /// Gets the option category metadata object for a category.
        /// </summary>
        /// <typeparam name="T">The type of the category to get</typeparam>
        /// <exception cref="System.ArgumentException"><typeparamref name="T"> is an invalid type.</exception>
        /// <exception cref="System.ArgumentNullException"><typeparamref name="type"> is null.</exception>
        /// <returns>The metadata instance for the category</returns>
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

        /// <summary>
        /// Saves all current changes to options into PlayerPrefs.
        /// </summary>
        public void SaveAllChanges() {
            foreach (OptionInfo option in Options) 
                option.Save();
        }

        /// <summary>
        /// Revert all crrent changes to the stored values in PlayerPrefs for all
        /// options in an assembly.
        /// If <paramref name="assembly"> is null, the current executing assembly is used.
        /// Will load all options from that assembly.
        /// </summary>
        /// <param name="assembly">the assembly to use</param>
        public void RevertAll(Assembly assembly = null) {
            LoadAllOptions(assembly);
            foreach (OptionInfo option in Options)
                option.Revert();
        }

        /// <summary>
        /// Revert all options to their default values for all options in an assembly.
        /// If <paramref name="assembly"> is null, the current executing assembly is used.
        /// Will load all options from that assembly.
        /// </summary>
        /// <param name="assembly">the assembly to use</param>
        public void ResetAll(Assembly assembly = null) {
            PlayerPrefs.DeleteAll();
            LoadAllOptions(assembly);
            foreach (OptionInfo option in Options)
                option.Revert();
        }

    }
}
