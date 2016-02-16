using UnityEngine;
using Hourai.Localization;

namespace Hourai.SmashBrew.UI {

    public class CharacterNameText : AbstractLocalizedText, IPlayerGUIComponent, ICharacterGUIComponent {

        [SerializeField]
        private CharacterData _character;

        [SerializeField]
        private bool shortName;

        private Player _player;
        private Character character;

        public void SetCharacter(CharacterData character) {
            if (!character)
                return;
            LocalizationKey = shortName ? character.ShortName : character.FullName;
        }

        public void SetPlayer(Player data) {
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
            _player = data;
            if (_player != null)
                _player.OnChanged += OnPlayerChange;
            OnPlayerChange();
        }

        void OnPlayerChange() {
            SetCharacter(_player == null ? null : _player.SelectedCharacter);
        }
    }

}
