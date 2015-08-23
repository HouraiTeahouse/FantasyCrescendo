using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(NumberText))]
    public class CharacterDamageDisplay : CharacterGUIComponent<CharacterDamage> {

        private Text _text;
        private NumberText _numberText;

        void Awake() {
            _text = GetComponent<Text>();
            _numberText = GetComponent<NumberText>();
        }

        void Update() {
            bool visible = Component != null;
            _text.enabled = visible;
            _numberText.enabled = visible;

            if (visible)
                _numberText.Number = Component.InternalDamage;
        }

    }
}

