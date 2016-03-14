using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// A UI Component that depends on data assigned from a Player object 
    /// </summary>
    public abstract class PlayerUIComponent : UIBehaviour, IDataComponent<Player> {
        private Player _player;

        /// <summary>
        /// The target Player the behaviour represents
        /// </summary>
        public Player Player {
            get { return _player; }
            set { SetData(value); }
        }

        /// <summary>
        /// Unity callback. Called on object destruction.
        /// </summary>
        protected override void OnDestroy() {
            base.OnDestroy();
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
        }

        /// <summary>
        /// Event callback. Called whenever <see cref="Player"/>'s state changes
        /// </summary>
        protected virtual void OnPlayerChange() {
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        /// <param name="data">the data to set</param>
        public virtual void SetData(Player data) {
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
            _player = data;
            if (_player != null)
                _player.OnChanged += OnPlayerChange;
            OnPlayerChange();
        }
    }

    /// <summary>
    /// An abstract UI behaviour class for handling a Character's data
    /// </summary>
    public abstract class CharacterUIComponent : PlayerUIComponent, IDataComponent<CharacterData> {
        [SerializeField, Tooltip("The character whose data is to be displayed")]
        private CharacterData _character;
        /// <summary>
        /// The target Character currently represented by the behaviour
        /// </summary>
        public CharacterData Character {
            get { return _character; }
            set { SetData(value); }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SetData(_character);
        }

        /// <summary>
        /// <see cref="PlayerUIComponent{T}.OnPlayerChange"/>
        /// </summary>
        protected override void OnPlayerChange() {
            SetData(Player == null ? null : Player.SelectedCharacter);
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public virtual void SetData(CharacterData data) {
            _character = data;
        }
    }

    /// <summary>
    /// An abstract UI behaviour class for handling a Player's current state
    /// </summary>
    /// <typeparam name="T">the type of component the PlayerUIComponent manipulates</typeparam>
    public abstract class PlayerUIComponent<T> : PlayerUIComponent where T : Component {
        [SerializeField] private T _component;

        /// <summary>
        /// The component the behaviour manipulates
        /// </summary>
        public T Component {
            get { return _component; }
            protected set { _component = value; }
        }

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            if (!_component)
                _component = GetComponent<T>();
        }
    }

    /// <summary>
    /// An abstract UI behaviour class for handling a Character's data
    /// </summary>
    /// <typeparam name="T">the type of component the CharacterUIComponent manipulates</typeparam>
    public abstract class CharacterUIComponent<T> : PlayerUIComponent<T>, IDataComponent<CharacterData>
        where T : Component {
        [SerializeField, Tooltip("The character whose data is to be displayed")] private CharacterData _character;

        /// <summary>
        /// The target Character currently represented by the behaviour
        /// </summary>
        public CharacterData Character {
            get { return _character; }
            set { SetData(value); }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SetData(_character);
        }

        /// <summary>
        /// <see cref="PlayerUIComponent{T}.OnPlayerChange"/>
        /// </summary>
        protected override void OnPlayerChange() {
            SetData(Player == null ? null : Player.SelectedCharacter);
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public virtual void SetData(CharacterData data) {
            _character = data;
        }
    }
}
