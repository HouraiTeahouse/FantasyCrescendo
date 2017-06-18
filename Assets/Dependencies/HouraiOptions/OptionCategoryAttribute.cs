using System;

namespace HouraiTeahouse.Options {

    /// <summary>
    /// Denotes a type as an option category.
    /// See <see cref="HouraiTeahouse.Options.OptionsManager">.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OptionCategoryAttribute : Attribute {

        /// <summary>
        /// The human readable name of the category.
        /// </summary>
        public string Name { get; private set; }

        public OptionCategoryAttribute(string name = null) {
            Name = name;
        }

    }

}

