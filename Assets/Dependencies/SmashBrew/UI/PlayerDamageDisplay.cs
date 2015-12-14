using UnityEngine;

namespace Hourai.SmashBrew.UI {

    public class PlayerDamageDisplay : GradientNumberText, IPlayerGUIComponent {

        private Damage _damage;

        [SerializeField]
        private int suffixSize = 25;

        protected override void Update() {
            base.Update();
            if (!Text)
                return;
            //TODO: Change this into a event
            bool visible = _damage != null && _damage.isActiveAndEnabled;
            Text.enabled = visible;

            if (visible)
                Number = Mathf.Floor(_damage.CurrentDamage);
        }

        protected override string ProcessNumber(string number) {
            string suffix = "%";
            if (_damage != null)
                suffix = _damage.Suffix;
            return string.Format("{0}<size={1}>{2}</size>", number, suffixSize, suffix);
        }

        public void SetPlayerData(Player data) {
            if (data == null || data.SpawnedCharacter == null) {
                _damage = null;
                return;
            }
            _damage = data.SpawnedCharacter.GetComponent<Damage>();
        }

    }
}

