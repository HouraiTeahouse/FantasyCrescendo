using UnityEngine;
using Hourai.Localization;

namespace Hourai.SmashBrew.UI {

    public class CharacterNameText : AbstractLocalizedText, ICharacterGUIComponent {

        private AbstractLocalizedText _abstractLocalizedText;

        [SerializeField]
        private bool shortName;

        private Character character;

        public void SetCharacter(CharacterData character) {
            if (!character)
                return;
            LocalizationKey = shortName ? character.ShortName : character.FullName;
        }

    }

}