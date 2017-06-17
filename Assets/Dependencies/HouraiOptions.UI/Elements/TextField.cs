using UnityEngine;

namespace HouraiTeahouse.Options.UI {

    public class TextField : AbstractOptionViewAttribute {

        public override void BuildUI(OptionInfo option, GameObject element) {
            var control = element.GetComponentInChildren<UnityEngine.UI.InputField>();
            control.text = option.GetPropertyValue<string>();
            control.onValueChanged.AddListener(value => option.SetPropertyValue(value));
        }

    }

}