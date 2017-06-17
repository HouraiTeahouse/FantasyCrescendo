using System;

namespace HouraiTeahouse.Options {

    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute {

        public string Name { get; private set; }
        public object DefaultValue { get; set; }

        public OptionAttribute(string name = null) {
            Name = name;
        }

    }

}
