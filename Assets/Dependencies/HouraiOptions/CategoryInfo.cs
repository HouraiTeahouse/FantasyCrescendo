using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.Options {

    public sealed class CategoryInfo {

        public object Instance { get; private set; }
        public Type Type { get; private set; }
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

        public IEnumerable<OptionInfo> Options {
            get { return _options.Values; }
        }

        public OptionInfo GetInfo(string name) {
            return _options[name];
        }

    }

}