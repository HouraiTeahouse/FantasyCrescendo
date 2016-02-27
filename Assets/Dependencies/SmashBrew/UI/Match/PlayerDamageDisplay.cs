using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerDamageDisplay : GradientNumberText, IPlayerGUIComponent {

        private Character _character;

        [SerializeField]
        private int suffixSize = 25;

        protected override void Update() {
            base.Update();
            if (!Text)
                return;
            //TODO: Change this into a event
            bool visible = _character != null && _character.isActiveAndEnabled;
            Text.enabled = visible;

            if (visible)
                Number = Mathf.Floor(_character.CurrentDamage);
        }

        protected override string ProcessNumber(string number) {
            string suffix = "%";
            if (_character != null)
                suffix = _character.DamageType.Suffix;
            return string.Format("{0}<size={1}>{2}</size>", number, suffixSize, suffix);
        }

        public void SetPlayer(Player data) {
            if (data == null || data.PlayerObject == null) {
                _character = null;
                return;
            }
            _character = data.PlayerObject;
        }

    }
}

