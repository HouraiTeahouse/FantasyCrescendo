using System;

namespace HouraiTeahouse.Options {

    /// <summary>
    /// Denotes a single property as an option within a category.
    /// See <see cref="HouraiTeahouse.Options.OptionsManager"> and
    /// <see cref="HouraiTeahouse.Options.OptionCategoryAttribute">.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute {

        /// <summary>
        /// Gets the human readable name of the option.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Get or set the default value for the option.
        /// </summary>
        /// <returns></returns>
        public object DefaultValue { get; set; }

        public OptionAttribute(string name = null) {
            Name = name;
        }

    }

}
