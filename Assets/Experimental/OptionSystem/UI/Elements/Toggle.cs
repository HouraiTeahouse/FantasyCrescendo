using UnityEngine;

namespace HouraiTeahouse.Options.UI {

    public class Toggle : AbstractOptionViewAttribute {

        public override void BuildUI(OptionInfo option, GameObject element) {
            var control = element.GetComponentInChildren<UnityEngine.UI.Toggle>();
            control.isOn = option.GetPropertyValue<bool>();
            control.onValueChanged.AddListener(value => option.SetPropertyValue(value));
        }

    }

}