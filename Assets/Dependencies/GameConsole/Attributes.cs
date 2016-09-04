using System;

namespace HouraiTeahouse.Console {

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute {

        public string Name { get; set; }

        public CommandAttribute(string name) { Name = name; }

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute {
    }

}
