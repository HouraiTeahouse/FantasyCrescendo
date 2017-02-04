using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse
{
    // A quick example of a Slider UI Builder for testing purposes only.
    public class UISliderBuilder : MonoBehaviour
    {
        [SerializeField]
        private OptionSystem optionSystem;
        private List<OptionSystem.CategoryInfo> categories;

        [SerializeField]
        private GameObject labelTemplate;
        [SerializeField]
        private GameObject sliderTemplate;

        void Start()
        {
            categories = optionSystem.MetadataList;
            for (int i = 0; i < categories.Count; i++)
            {
                var label = Instantiate(labelTemplate);
                label.name = categories[i].CategoryName;
                label.transform.parent = this.transform;
                label.GetComponent<Text>().text = categories[i].CategoryName;
                foreach (var option in categories[i].OptionList)
                {
                    if (option.OptionAttr is UISlider)
                    {
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

        void SliderValueToProperty(Slider slider, OptionSystem.OptionInfo info)
        {
            info.SetPropertyValue(slider.value);
        }
    }

    
}
