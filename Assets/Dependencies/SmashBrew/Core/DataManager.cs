using System.Collections.Generic;
using System.Collections.ObjectModel;
using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// A manager of all of the game data loaded into the game.
    /// </summary>
    public sealed class DataManager : MonoBehaviour {
        [SerializeField, Tooltip("Destroy this instance on scene loads?")] private bool _dontDestroyOnLoad;

        [SerializeField, Tooltip("The characters to display in the game")] private List<CharacterData> _characters;

        [SerializeField, Tooltip("The scenes to show in the game")] private List<SceneData> _scenes;

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
            get { return _characterCollection; }
        }

        /// <summary>
        /// All Scenes and their metadata included with the game's build.
        /// The DataManager will automatically load all SceneData instances from Resources.
        /// </summary>
        public ReadOnlyCollection<SceneData> Scenes {
            get { return _sceneCollection; }
        }

        /// <summary>
        /// Unity Callback. Called on object instantion.
        /// </summary>
        void Awake() {
            Instance = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);

            Mediator = GlobalMediator.Instance;

            _characterCollection = new ReadOnlyCollection<CharacterData>(_characters);
            _sceneCollection = new ReadOnlyCollection<SceneData>(_scenes);
        }

        void OnLevelWasLoaded(int level) {
            Debug.Log("Unload");
            foreach (SceneData scene in _scenes)
                scene.UnloadAll();
            foreach (CharacterData character in _characters)
                character.UnloadAll();
        }
    }
}
