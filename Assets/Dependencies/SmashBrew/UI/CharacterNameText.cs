using UnityEngine;
using Hourai.Localization;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(LocalizedText))]
    public class CharacterNameText : MonoBehaviour, ICharacterGUIComponent {

        private LocalizedText _localizedText;

        [SerializeField]
        private bool shortName;

        void Awake() {
            _localizedText = GetComponent<LocalizedText>();
        }

        public void SetCharacter(CharacterData character) {
            if (!_localizedText || !character)
                return;
            if (shortName)
                _localizedText.Key = character.ShortName;
            else
                _localizedText.Key = character.FullName;
        }

    }

}