using UnityEngine;

namespace HouraiTeahouse.Options.UI {

    public class IntField : AbstractOptionViewAttribute {

        public override void BuildUI(OptionInfo option, GameObject element) {
            var control = element.GetComponentInChildren<UnityEngine.UI.InputField>();
            control.text = option.GetPropertyValue<int>().ToString();
            control.onValueChanged.AddListener(value => option.SetPropertyValue(int.Parse(value)));
        }

    }

}