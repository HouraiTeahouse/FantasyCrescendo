using UnityEngine.EventSystems;

namespace Hourai.SmashBrew.UI {

    public abstract class PlayerCharacterUIComponent : UIBehaviour, IPlayerGUIComponent, ICharacterGUIComponent {

        private Player _player;

        public virtual void SetPlayer(Player data) {
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

        public abstract void SetCharacter(CharacterData data);
    }

}
