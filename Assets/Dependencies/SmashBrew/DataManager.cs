using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public sealed class DataManager : MonoBehaviour {

        [SerializeField]
        private bool _dontDestroyOnLoad;

        private ReadOnlyCollection<CharacterData> _characters;
        private ReadOnlyCollection<SceneData> _scenes;  

        public Mediator Mediator { get; private set; }

        /// <summary>
        /// The Singleton instance of DataManager.
        /// </summary>
        public static DataManager Instance { get; private set; }

        /// <summary>
        /// All Characters that are included with the Game's build.
        /// The Data Manager will automatically load all CharacterData instances from Resources.
        /// </summary>
        public ICollection<CharacterData> Characters {
            get {
                if (_characters == null)
                    _characters = new ReadOnlyCollection<CharacterData>(Resources.LoadAll<CharacterData>(string.Empty));
                return _characters;
            }
        }

        /// <summary>
        /// All Scenes and their metadata included with the game's build.
        /// The DataManager will automatically load all SceneData instances from Resources.
        /// </summary>
        public ICollection<SceneData> Scenes {
            get {
                if(_scenes == null)
                   _scenes = new ReadOnlyCollection<SceneData>(Resources.LoadAll<SceneData>(string.Empty));
                return _scenes;
            }
        }

        /// <summary>
        /// Unity Callback. Called on object instantion.
        /// </summary>
        void Awake() {
            Instance = this;

            if(_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
            
            Mediator = GlobalMediator.Instance;
            Mediator.Subscribe<DataEvent.ChangePlayerLevelCommand>(OnChangePlayerLevel);
            Mediator.Subscribe<DataEvent.ChangePlayerMode>(OnChangePlayerMode);
            Mediator.Subscribe<DataEvent.UserChangingOptions>(OnUserChangingOptions);
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            if (Mediator == null)
                return;
            Mediator.Unsubscribe<DataEvent.ChangePlayerLevelCommand>(OnChangePlayerLevel);
            Mediator.Unsubscribe<DataEvent.ChangePlayerMode>(OnChangePlayerMode);
            Mediator.Unsubscribe<DataEvent.UserChangingOptions>(OnUserChangingOptions);
        }

        /// <summary>
        /// Check if the battle can start, that is when there is at least
        /// two players or CPUs ready to battle.
        /// </summary>
        /// <returns><c>true</c>, if the battle can start, <c>false</c> otherwise.</returns>
        public bool IsReadyToStartGame() {
            int counter = Player.ActivePlayerCount;
            Mediator.Publish(new DataEvent.ReadyToFight { IsReady = (counter > 1) });
            return (counter > 1);
        }

        void OnChangePlayerLevel(DataEvent.ChangePlayerLevelCommand cmd) {
            cmd.Player.CPULevel = cmd.NewLevel;
        }

        void OnUserChangingOptions(DataEvent.UserChangingOptions cmd) {
            //Debug.Log ("Ai mano " + cmd.isUserChangingOptions  + "  -  " + Time.time );
            if (cmd.IsUserChangingOptions) {
                Mediator.Publish(
                                 new DataEvent.ReadyToFight { IsReady = false });
            } else
                IsReadyToStartGame();
        }

        void OnChangePlayerMode(DataEvent.ChangePlayerMode cmd) {
            cmd.Player.CycleType();

            IsReadyToStartGame();
        }

    }
}
