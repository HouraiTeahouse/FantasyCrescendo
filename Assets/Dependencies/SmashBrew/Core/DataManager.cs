using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using HouraiTeahouse.AssetBundles;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A manager of all of the game data loaded into the game. </summary>
    public sealed class DataManager : Singleton<DataManager> {

        static readonly ILog log = Log.GetLogger<DataManager>();

        List<SceneData> _scenes;
        List<CharacterData> _characterList;
        Dictionary<uint, CharacterData> _characters;
        public ITask LoadTask { get; private set; }

        public bool IsReady {
            get { return LoadTask != null && LoadTask.State != TaskState.Pending; }
        }

        /// <summary> All Characters that are included with the Game's build. The Data Manager will automatically load all
        /// CharacterData instances from Resources. </summary>
        public ReadOnlyCollection<CharacterData> Characters { get; private set; }

        /// <summary> All Scenes and their metadata included with the game's build. The DataManager will automatically load all
        /// SceneData instances from Resources. </summary>
        public ReadOnlyCollection<SceneData> Scenes { get; private set; }

        /// <summary> Unity Callback. Called on object instantion. </summary>
        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(this);

            _characterList = new List<CharacterData>();
            _characters = new Dictionary<uint, CharacterData>();
            _scenes = new List<SceneData>();

            AssetBundleManager.Initialize();

            AssetBundleManager.AddHandler<CharacterData>(AddCharacter);
            AssetBundleManager.AddHandler<SceneData>(AddScene);

#if UNITY_EDITOR
            if (AssetBundleManager.SimulateAssetBundleInEditor) {
                LoadFromEditor<CharacterData>(AddCharacter);
                LoadFromEditor<SceneData>(AddScene);
                LoadTask = Task.Resolved;
            }
            else
#endif
            {
                var bundlePath = BundleUtility.GetStoragePath();
                var dataFilePath = Path.Combine(bundlePath, "data");
                log.Info("Storage Path: {0}", bundlePath);
                if (!File.Exists(dataFilePath)) {
                    log.Info("Cannot find {0} copying StreamingAssets...", dataFilePath, bundlePath);
                    AssetBundleManager.CopyDirectory(Application.streamingAssetsPath, bundlePath);
                }

                var file = File.ReadAllText(dataFilePath);
                var whitelist = new List<string>();
                var blacklist = new List<string>();
                foreach (var entry in file.Split('\n')) {
                    if (entry.StartsWith("-")) {
                        blacklist.Add(entry.Substring(1).Trim());
                        log.Info("Registered bundle blacklist: {0}".With(entry.Substring(1).Trim()));
                    } else {
                        whitelist.Add(entry.Trim());
                        log.Info("Registered bundle whitelist: {0}".With(entry.Trim()));
                    }
                }

                LoadTask = AssetBundleManager.LoadLocalBundles(whitelist, blacklist).Then(() => {
                });
                LoadTask.Done();
            }

            Characters = new ReadOnlyCollection<CharacterData>(_characterList);
            Scenes = new ReadOnlyCollection<SceneData>(_scenes);



            SceneManager.sceneLoaded += SceneLoad;
        }

        void OnDestroy() {
            AssetBundleManager.RemoveHandler<CharacterData>(AddCharacter);
            AssetBundleManager.RemoveHandler<SceneData>(AddScene);
        }

#if UNITY_EDITOR
        void LoadFromEditor<T>(Action<T> loadFunc) where T : Object {
            var guids = AssetDatabase.FindAssets("t:{0}".With(typeof(T).Name));
            foreach (string guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    loadFunc(asset);
            }
        }
#endif

        public void AddCharacter(CharacterData data) {
            if (_characters.ContainsKey(data.Id)) {
                log.Warning("Attempted to load {0} while already loaded.", data);
                return;
            }
            _characters[data.Id] = data;
            _characterList.Add(data);
            //data.Prefab.LoadAsync().Then(prefab => {
            //    ClientScene.RegisterPrefab(prefab);
            //    log.Info("Registered {0} ({1}) as a spawnable network character.", data.name, data.Id);
            //});
            log.Info("Registered {0} ({1}) as a valid character.", data.name, data.Id);
        }

        public void AddScene(SceneData data) {
            if (_scenes.Contains(data)) {
                log.Warning("Attempted to load {0} while already loaded.", data);
                return;
            }
            _scenes.Add(data);
            log.Info("Registered {0} as a valid scene.", data.name);
        }

        void SceneLoad(Scene newScene, LoadSceneMode mode) {
            Log.Info("Unloading managed data assets");
            foreach (SceneData scene in _scenes)
                scene.Unload();
            foreach (CharacterData character in _characters.Values)
                character.Unload();
        }

        /// <summary>
        /// Looks up a character by it's ID. Returns null if no character is found.
        /// </summary>
        /// <param name="id">the id of the character to lookup</param>
        /// <returns>The matching character</returns>
        public CharacterData GetCharacter(uint id) {
            if (_characters .ContainsKey(id))
                return _characters[id];
            return null;
        }

    }

}
