using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public sealed class DataManager : MonoBehaviour {

        [SerializeField]
        private bool _dontDestroyOnLoad;

        [SerializeField]
        private List<CharacterData> _characters;

        [SerializeField]
        private List<SceneData> _scenes;

        private ReadOnlyCollection<CharacterData> _characterCollection;
        private ReadOnlyCollection<SceneData> _sceneCollection;  

        public Mediator Mediator { get; private set; }

        /// <summary>
        /// The Singleton instance of DataManager.
        /// </summary>
        public static DataManager Instance { get; private set; }

        /// <summary>
        /// All Characters that are included with the Game's build.
        /// The Data Manager will automatically load all CharacterData instances from Resources.
        /// </summary>
        public ReadOnlyCollection<CharacterData> Characters {
            get {
                return _characterCollection;
            }
        }

        /// <summary>
        /// All Scenes and their metadata included with the game's build.
        /// The DataManager will automatically load all SceneData instances from Resources.
        /// </summary>
        public ReadOnlyCollection<SceneData> Scenes {
            get {
                return _sceneCollection;
            }
        }

        /// <summary>
        /// Unity Callback. Called on object instantion.
        /// </summary>
        void Awake() {
            Instance = this;

            if(_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
           
            _characterCollection = new ReadOnlyCollection<CharacterData>(_characters);
            _sceneCollection = new ReadOnlyCollection<SceneData>(_scenes);

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
