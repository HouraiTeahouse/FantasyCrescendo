using UnityEngine;
using Hourai.Localization;

namespace Hourai.SmashBrew.UI {

    [DisallowMultipleComponent]
    public class CharacterNameDisplay : AbstractLocalizedText, IPlayerGUIComponent {

        [SerializeField]
        private bool toUpperCase = true;

        public void SetPlayer(Player data) {
            if (Text == null)
                return;

            if (data == null || data.SelectedCharacter == null)
                Text.text = "";
            else
                LocalizationKey = data.SelectedCharacter.ShortName;
        }

        protected override string Process(string raw) {
            if (toUpperCase)
                raw = raw.ToUpper();
            return raw;
        }

    }

}
