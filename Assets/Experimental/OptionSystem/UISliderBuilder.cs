using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse
{
    // A quick example of a Slider UI Builder for testing purposes only.
    public class UISliderBuilder : MonoBehaviour {

        [SerializeField]
        OptionSystem _optionSystem;
         _categories;

        [SerializeField]
        GameObject labelTemplate;
        [SerializeField]
        GameObject sliderTemplate;

        void Start() {
            foreach (OptionSystem.CategoryInfo category in _optionSystem.MetadataList) {
                var label = Instantiate(labelTemplate);
                label.name = category.CategoryName;
                label.transform.parent = this.transform;
                label.GetComponent<Text>().text = category.CategoryName;
                foreach (var option in category.OptionList) {
                    if (option.OptionAttr is UISlider) {
                        UISlider sliderAttr = (UISlider)option.OptionAttr;
                        var sliderObj = Instantiate(sliderTemplate).GetComponent<Slider>();
                        sliderObj.gameObject.name = sliderAttr.Name;
                        sliderObj.transform.parent = label.transform;
                        sliderObj.minValue = sliderAttr.Min;
                        sliderObj.maxValue = sliderAttr.Max;
                        sliderObj.value = (float)option.GetPropertyValue();
                        sliderObj.onValueChanged.AddListener(delegate { SliderValueToProperty(sliderObj, option); });
                    }
                }
            }
        }

        void SliderValueToProperty(Slider slider, OptionSystem.OptionInfo info) {
            info.SetPropertyValue(slider.value);
        }
    }

    
}
