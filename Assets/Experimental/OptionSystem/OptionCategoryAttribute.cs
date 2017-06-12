using System;

namespace HouraiTeahouse.Options {

    [AttributeUsage(AttributeTargets.Class)]
    public class OptionCategoryAttribute : Attribute {

        public string Name { get; private set; }

        public OptionCategoryAttribute(string name = null) {
            Name = name;
        }

    }

}

