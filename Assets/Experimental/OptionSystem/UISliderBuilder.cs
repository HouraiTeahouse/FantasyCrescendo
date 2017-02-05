using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse
{
    // A quick example of a Slider UI Builder for testing purposes only.
    public class UISliderBuilder : MonoBehaviour {

        [SerializeField]
        OptionSystem optionSystem;

        [SerializeField]
        GameObject labelTemplate;
        [SerializeField]
        GameObject sliderTemplate;

        void Start() {
            foreach (CategoryInfo category in optionSystem.Categories) {
                var label = Instantiate(labelTemplate);
                label.name = category.Name;
                label.transform.SetParent(transform, false);
                label.GetComponent<Text>().text = category.Name;
                foreach (OptionInfo option in category.Options) {
                    if (option.Attribute is UISlider) {
                        UISlider sliderAttr = (UISlider)option.Attribute;
                        var sliderObj = Instantiate(sliderTemplate).GetComponent<Slider>();
                        sliderObj.gameObject.name = sliderAttr.Name;
                        sliderObj.transform.SetParent(label.transform, false);
                        sliderObj.minValue = sliderAttr.Min;
                        sliderObj.maxValue = sliderAttr.Max;
                        sliderObj.value = (float)option.GetPropertyValue();
                        sliderObj.onValueChanged.AddListener(delegate { SliderValueToProperty(sliderObj, option); });
                    }
                }
            }
        }

        void SliderValueToProperty(Slider slider, OptionInfo info) {
            info.SetPropertyValue(slider.value);
        }
    }

    
}
