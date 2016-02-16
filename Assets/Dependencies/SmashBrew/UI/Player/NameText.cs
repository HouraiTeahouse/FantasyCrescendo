using UnityEngine;
using Hourai.Localization;

namespace Hourai.SmashBrew.UI {

    public class NameText : AbstractLocalizedText, IPlayerGUIComponent, ICharacterGUIComponent{

        [SerializeField]
        private CharacterData _character;

        [SerializeField]
        private bool shortName;

        [SerializeField]
        private bool _capitalize;

        private Player _player;
        private Character character;

        public void SetCharacter(CharacterData data) {
            if (data == null)
                Text.text = string.Empty;
            else
                LocalizationKey = shortName ? data.ShortName : data.FullName;
        }

        public void SetPlayer(Player data) {
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
            _player = data;
            if (_player != null)
                _player.OnChanged += OnPlayerChange;
            OnPlayerChange();
        }

        protected override string Process(string val) {
            return !_capitalize ? val : val.ToUpperInvariant();
        }

        void OnPlayerChange() {
            SetCharacter(_player == null ? null : _player.SelectedCharacter);
        }
    }

}
