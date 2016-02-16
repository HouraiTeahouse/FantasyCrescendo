using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public abstract class PlayerUIComponent<T> : UIBehaviour, IPlayerGUIComponent where T : Component {

        private Player _player;

        [SerializeField]
        private T _component;

        public T Component {
            get { return _component; }
            protected set { _component = value; }
        }

        public Player Player {
            get { return _player; }
            set { SetPlayer(value); }
        }

        protected override void Awake() {
            base.Awake();
            if (!_component)
                _component = GetComponent<T>();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
        }

        public virtual void SetPlayer(Player data) {
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
            _player = data;
            if (_player != null)
                _player.OnChanged += OnPlayerChange;
            OnPlayerChange();
        }

        protected virtual void OnPlayerChange() {
        }
    }

    public abstract class CharacterUIComponent<T> : PlayerUIComponent<T>, ICharacterGUIComponent where T : Component {

        [SerializeField]
        private CharacterData _character;

        public CharacterData Character {
            get { return _character; }
            set { SetCharacter(value); }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SetCharacter(_character); 
        }

        protected override void OnPlayerChange() {
            SetCharacter(Player == null ? null : Player.SelectedCharacter);
        }

        public virtual void SetCharacter(CharacterData data) {
            _character = data;
        }
    }
}
