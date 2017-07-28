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
            var valueIndex = GetInitialIndex(option.GetPropertyValue<string>());
            if (valueIndex >= 0) {
                control.value = valueIndex;
            } else {
                control.value = 0;
                SaveValue(option, Options[0], 0);
            }
            control.onValueChanged.AddListener(index => {
                if (index < 0 || index >= Options.Count) {
                    control.value = 0;
                    return;
                }
                SaveValue(option, Options[index], index);
            });
        }

        public virtual int GetInitialIndex(string value) {
            return Options.IndexOf(value);
        }

        public virtual void SaveValue(OptionInfo option, string selectionValue, int index) {
            option.SetPropertyValue(selectionValue);
        }

    }

}