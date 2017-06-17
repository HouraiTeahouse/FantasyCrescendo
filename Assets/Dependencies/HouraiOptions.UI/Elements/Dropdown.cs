using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.Options.UI {

    public class Dropdown : AbstractOptionViewAttribute {

        public virtual List<string> Options { get; set; }

        public Dropdown(params string[] options) {
            Options = options.ToList();
        }

        public override void BuildUI(OptionInfo option, GameObject element) {
            var control = element.GetComponentInChildren<UnityEngine.UI.Dropdown>();
            control.options = Options.Select(opt => new UnityEngine.UI.Dropdown.OptionData(opt)).ToList();
            var valueIndex = Options.IndexOf(option.GetPropertyValue<string>());
            if (valueIndex >= 0) {
                control.value = valueIndex;
            } else {
                control.value = 0;
                option.SetPropertyValue(Options[0]);
            }
            control.onValueChanged.AddListener(index => {
                if (index < 0 || index >= Options.Count) {
                    control.value = 0;
                    return;
                }
                option.SetPropertyValue(Options[index]);
            });
        }

    }

}