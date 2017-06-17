using UnityEngine;

namespace HouraiTeahouse.Options.UI {

    public class Slider : AbstractOptionViewAttribute {

        public float Min { get; private set; }
        public float Max { get; private set; }

        public Slider(float min, float max)  {
            Min = min;
            Max = max;
        }

        public override void BuildUI(OptionInfo option, GameObject element) {
            var sliderObj = element.GetComponentInChildren<UnityEngine.UI.Slider>();
            sliderObj.minValue = Min;
            sliderObj.maxValue = Max;
            sliderObj.value = option.GetPropertyValue<float>();
            sliderObj.onValueChanged.AddListener(value => option.SetPropertyValue(value));
        }

    }

}