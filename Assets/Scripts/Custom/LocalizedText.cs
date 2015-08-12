using UnityEngine;
using UnityEngine.UI;

#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_1_0

namespace SmartLocalization.Editor {

    [RequireComponent(typeof (Text))]
    public class LocalizedText : MonoBehaviour {

        public string localizedKey = "INSERT_KEY_HERE";
        private Text textObject;

        private void Start() {
            textObject = GetComponent<Text>();

            //Subscribe to the change language event
            LanguageManager languageManager = LanguageManager.Instance;
            languageManager.OnChangeLanguage += OnChangeLanguage;

            //Run the method one first time
            OnChangeLanguage(languageManager);
        }

        private void OnDestroy() {
            if (LanguageManager.HasInstance)
                LanguageManager.Instance.OnChangeLanguage -= OnChangeLanguage;
        }

        private void OnChangeLanguage(LanguageManager languageManager) {
            textObject.text = LanguageManager.Instance.GetTextValue(localizedKey);
        }

    }

}

#endif