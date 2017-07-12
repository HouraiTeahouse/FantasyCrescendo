using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Options.UI {

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractOptionViewAttribute : Attribute {

        public abstract void BuildUI(OptionInfo option, GameObject element);

    }

}
