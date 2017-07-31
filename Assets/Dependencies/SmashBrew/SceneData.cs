using HouraiTeahouse.AssetBundles;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse.SmashBrew {

    public interface IGameData {

        // Is the data selectable?
        bool IsSelectable { get; }

        // Is the data visible 
        bool IsVisible { get; }

        // Unloads all data associated with the data
        void Unload();

    }

    public enum SceneType {
        Other = 0,
        Stage = 1,
        Menu = 2
    }

    [CreateAssetMenu(fileName = "New Stage", menuName = "SmashBrew/Scene Data")]
    public class SceneData : BGMGroup, IGameData {

        [Header("Load Data")]
        [SerializeField]
        [Tooltip("What kind of scene is it?")]
        SceneType _type;

        [SerializeField]
        [Tooltip("The priority in loading in dynamic enviroments.")]
        int _loadPriority;

        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The icon used shown on menus to represent the scene.")]
        string _icon;

        [SerializeField]
        [Tooltip("Is this scene selectable on select screens?")]
        bool _isSelectable = true;

        [SerializeField]
        [Tooltip("Is this scene visible on select screens?")]
        bool _isVisible = true;

        [SerializeField]
        bool _isDebug;

        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The image shown on menus to represent the scene.")]
        string _previewImage;

        /// <summary> The internal name of the scene. Must be in build settings. </summary>
        [SerializeField]
        [Scene]
        [Tooltip("The internal name of the scene. Must be in build settings.")]
        string _scene;

        /// <summary> The image shown on menus to represent the scene. </summary>
        public Resource<Sprite> PreviewImage { get; private set; }

        /// <summary> The icon used on menus to represent the scene. </summary>
        public Resource<Sprite> Icon { get; private set; }

        /// <summary> Is the scene described by this SceneData a stage? </summary>
        public SceneType Type {
            get { return _type; }
        }

        public int LoadPriority {
            get { return _loadPriority; }
        }

        public bool IsSelectable {
            get { return _isSelectable && IsVisible; }
        }

        public bool IsVisible {
            get { 
                if (!Debug.isDebugBuild)
                    return !_isDebug && _isVisible;
                return _isVisible; 
            }
        }

        public void Unload() {
            if(PreviewImage != null)
                PreviewImage.Unload();
            if (Icon != null)
                Icon.Unload();
        }

        /// <summary> Loads the scene described by the SceneData </summary>
        public ITask Load() {
            var task = SceneLoader.LoadScene(_scene);
            task.Then(() =>
                Mediator.Global.Publish(new LoadSceneEvent {
                    Scene = this
                })
            );
            return task;
        }

        /// <summary> Unity Callback. Called when ScriptableObject is loaded. </summary>
        protected override void OnEnable() {
            base.OnEnable();
            PreviewImage = Resource.Get<Sprite>(_previewImage);
            Icon = Resource.Get<Sprite>(_icon);
        }

        /// <summary> Unity callback. Called when ScriptableObject is unloaded. </summary>
        protected override void OnDisable() {
            base.OnDisable();
            Unload();
        }

    }

}
