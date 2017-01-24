using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A UI Text driver that displays the current damage of the a given player. </summary>
    public sealed class PlayerDamageDisplay : GradientNumberText, IDataComponent<Player> {

        Player _player;
        DamageState _damage;

        [SerializeField]
        [Tooltip("The font size of the suffix")]
        int suffixSize = 25;

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            if (_player != null)
                _player.Changed -= PlayerChange;
            _player = data;
            if (_player != null)
                _player.Changed += PlayerChange;
            PlayerChange();
        }

        void PlayerChange() {
            if (_player == null || _player.PlayerObject == null)
                _damage = null;
            else {
                _damage = _player.PlayerObject.GetComponent<DamageState>();
                Number = _damage;
            }
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        protected override void Update() {
            base.Update();
            if (!Text || !_damage)
                return;
            //TODO: Change this into a event
            bool visible = _damage.isActiveAndEnabled;
            Text.enabled = visible;
            float value = Mathf.Floor(_damage);
            if (visible && !Mathf.Approximately(Number, value))
                Number = value;
        }

        /// <summary>
        ///     <see cref="NumberText.ProcessNumber" />
        /// </summary>
        protected override string ProcessNumber(string number) {
            if (!_damage)
                return number;
            return string.Format("{0}<size={1}>{2}</size>", number, suffixSize, _damage.Type.Suffix);
        }

    }

}
