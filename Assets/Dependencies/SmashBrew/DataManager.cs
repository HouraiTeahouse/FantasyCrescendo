using System.Collections.Generic;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class DataManager : MonoBehaviour {

        private CharacterData[] _availableCharacters;
        private List<string> _availableStages;
        public Mediator Mediator;

        private static DataManager _instance;

        public static DataManager Instance {
            get { return _instance; }
        }

        /// <summary>
        /// Unity Callback. Called on object instantion.
        /// </summary>
        void Awake() {
            _instance = this;
            _availableCharacters = Resources.LoadAll<CharacterData>("");

            _availableStages = new List<string> {"Hakurei Shrine", "Marisa's House"};

            //TODO: Convert these into a SceneData ScriptableObject
            InitializeMediator();
        }

        /// <summary>
        /// Check if the battle can start, that is when there is at least
        /// two players or CPUs ready to battle.
        /// </summary>
        /// <returns><c>true</c>, if the battle can start, <c>false</c> otherwise.</returns>
        public bool IsReadyToStartGame() {
            int counter = SmashGame.ActivePlayerCount;
            Mediator.Publish(new DataEvent.ReadyToFight { IsReady = (counter > 1) });
            return (counter > 1);
        }

        public void InitializeMediator() {
            Mediator = GlobalMediator.Instance;
            Mediator.Subscribe<DataEvent.ChangePlayerLevelCommand>(OnChangePlayerLevel);
            Mediator.Subscribe<DataEvent.ChangePlayerMode>(OnChangePlayerMode);
            Mediator.Subscribe<DataEvent.UserChangingOptions>(OnUserChangingOptions);
        }

        public List<string> GetAvailableStages() {
            return _availableStages;
        }

        public IEnumerable<CharacterData> GetAvailableCharacters() {
            foreach (CharacterData data in _availableCharacters)
                yield return data;
        }

        public void OnChangePlayerLevel(DataEvent.ChangePlayerLevelCommand cmd) {
            cmd.Player.CpuLevel = cmd.NewLevel;
        }

        public void OnUserChangingOptions(DataEvent.UserChangingOptions cmd) {
            //Debug.Log ("Ai mano " + cmd.isUserChangingOptions  + "  -  " + Time.time );
            if (cmd.IsUserChangingOptions) {
                Mediator.Publish(
                                 new DataEvent.ReadyToFight { IsReady = false });
            } else
                IsReadyToStartGame();
        }

        public void OnChangePlayerMode(DataEvent.ChangePlayerMode cmd) {
            cmd.Player.CycleType();

            IsReadyToStartGame();
        }

    }
}
