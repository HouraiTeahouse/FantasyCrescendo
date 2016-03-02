using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary>
    /// A UI Text driver that displays the current damage of the a given player.
    /// </summary>
    public sealed class PlayerDamageDisplay : GradientNumberText, IDataComponent<Player> {

        private Character _character;

        [SerializeField, Tooltip("The font size of the suffix")]
        private int suffixSize = 25;

        /// <summary>
        /// Unity callback. Called once per frame.
        /// </summary>
        protected override void Update() {
            base.Update();
            if (!Text || !_character)
                return;
            //TODO: Change this into a event
            bool visible = _character.isActiveAndEnabled;
            Text.enabled = visible;
            float value = Mathf.Floor(_character.CurrentDamage);
            if (visible && !Mathf.Approximately(Number, value))
                Number = value;
        }

        /// <summary>
        /// <see cref="NumberText.ProcessNumber"/>
        /// </summary>
        protected override string ProcessNumber(string number) {
            if (!_character)
                return number;
            return string.Format("{0}<size={1}>{2}</size>", number, suffixSize, _character.DamageType.Suffix);
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public void SetData(Player data) {
            if (data == null || data.PlayerObject == null)
                _character = null;
            else {
                _character = data.PlayerObject;
                Number = _character.CurrentDamage;
            }
        }

    }
}

