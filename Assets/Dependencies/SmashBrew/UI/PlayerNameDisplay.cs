using UnityEngine;
using Hourai.Localization;

namespace Hourai.SmashBrew.UI {

    [DisallowMultipleComponent]
    public class PlayerNameDisplay : LocalizedText, IPlayerGUIComponent {

        [SerializeField]
        private bool toUpperCase = true;


        public void SetPlayerData(Player data) {
            if (Text == null)
                return;

            if (data == null || data.Character == null)
                Text.text = "";
            else
                Key = data.Character.FirstName;
        }

        protected override string Process(string raw) {
            if (toUpperCase)
                raw = raw.ToUpper();
            return raw;
        }

    }

}