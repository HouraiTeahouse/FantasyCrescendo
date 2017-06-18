using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.Options {

    /// <summary>
    /// Metadata object for a category of options.
    /// See <see cref="HouraiTeahouse.Options.OptionsManager">.
    /// </summary>
    public sealed class CategoryInfo {

        /// <summary>
        /// The object instance holding the current values of the category's
        /// options. Will be of the type described by <see cref="Type">.
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// The underlying type definition for the option category.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The human readable name of the category.
        /// </summary>
        public string Name { get; private set; }
        readonly Dictionary<string, OptionInfo> _options;

        internal CategoryInfo(object instance) {
            _options = new Dictionary<string, OptionInfo>();
            Instance = instance;
            Type = Argument.NotNull(instance).GetType();
            var categoryAttribute = Type.GetCustomAttributes(true).OfType<OptionCategoryAttribute>().FirstOrDefault();
            if (categoryAttribute != null) {
                Name = categoryAttribute.Name ?? Type.Name;
            }

            foreach (var prop in Type.GetProperties()) {
                var attribute = prop.GetCustomAttributes(true).OfType<OptionAttribute>().FirstOrDefault();
                if (attribute != null)
                    _options.Add(prop.Name, new OptionInfo(this, attribute, Type, prop));
            }
        }

        /// <summary>
        /// Gets an enumeration of OptionInfo describing all the 
        /// options under the category.
        /// </summary>
        public IEnumerable<OptionInfo> Options {
            get { return _options.Values; }
        }

        /// <summary>
        /// Gets a single OptionInfo for a specific option.
        /// </summary> 
        /// <param name="name">the name of the option to fetch</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///   Category does not contain an option of name <paramref name="name">
        /// </exception>
        public OptionInfo GetInfo(string name) {
            return _options[name];
        }

    }

}