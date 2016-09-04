using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A manager of all of the game data loaded into the game. </summary>
    public sealed class DataManager : MonoBehaviour {

        ReadOnlyCollection<CharacterData> _characterCollection;

        [SerializeField]
        [Tooltip("The characters to display in the game")]
        List<CharacterData> _characters;

        [SerializeField]
        [Tooltip("Destroy this instance on scene loads?")]
        bool _dontDestroyOnLoad;

        ReadOnlyCollection<SceneData> _sceneCollection;

        [SerializeField]
        [Tooltip("The scenes to show in the game")]
        List<SceneData> _scenes;

        public Mediator Mediator { get; private set; }

        /// <summary> The Singleton instance of DataManager. </summary>
        public static DataManager Instance { get; private set; }

        /// <summary> All Characters that are included with the Game's build. The Data Manager will automatically load all
        /// CharacterData instances from Resources. </summary>
        public ReadOnlyCollection<CharacterData> Characters {
            get { return _characterCollection; }
        }

        /// <summary> All Scenes and their metadata included with the game's build. The DataManager will automatically load all
        /// SceneData instances from Resources. </summary>
        public ReadOnlyCollection<SceneData> Scenes {
            get { return _sceneCollection; }
        }

        /// <summary> Unity Callback. Called on object instantion. </summary>
        void Awake() {
            Instance = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);

            Mediator = Mediator.Global;

            _characters = _characters.IgnoreNulls().ToList();
            _scenes = _scenes.IgnoreNulls().ToList();

            _characterCollection = new ReadOnlyCollection<CharacterData>(_characters);
            _sceneCollection = new ReadOnlyCollection<SceneData>(_scenes);

            SceneManager.sceneLoaded += SceneLoad;
        }

        void SceneLoad(Scene newScene, LoadSceneMode mode) {
            Log.Info("Unloading managed data assets");
            foreach (SceneData scene in _scenes)
                scene.Unload();
            foreach (CharacterData character in _characters)
                character.Unload();
        }

    }

}
