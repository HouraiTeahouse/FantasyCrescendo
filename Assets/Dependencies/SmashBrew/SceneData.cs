using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hourai.SmashBrew {

    public class SceneData : BGMGroup {

        /// <summary>
        /// The internal name of the scene. Must be in build settings. 
        /// </summary>
        [SerializeField]
        [Tooltip("The internal name of the scene. Must be in build settings.")]
        private string _sceneName;

        [SerializeField]
        [Tooltip("Is this scene a stage?")]
        private bool _isStage = true;

        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The image shown on menus to represent the scene.")]
        private string _previewImage;

        [SerializeField]
        [Resource(typeof (Sprite))]
        [Tooltip("The icon used shown on menus to represent the scene.")]
        private string _icon;

        /// <summary>
        /// The image shown on menus to represent the scene.
        /// </summary>
        public Resource<Sprite> PreviewImage { get; private set; }

        /// <summary>
        /// The icon used on menus to represent the scene.
        /// </summary>
        public Resource<Sprite> Icon { get; private set; }

        /// <summary>
        /// Is the scene described by this SceneData a stage?
        /// </summary>
        public bool IsStage {
            get { return _isStage; }
        }

        /// <summary>
        /// Unity Callback. Called when ScriptableObject is loaded.
        /// </summary>
        protected override void OnEnable() {
            base.OnEnable();
            PreviewImage = new Resource<Sprite>(_previewImage);
            Icon = new Resource<Sprite>(_icon);
            Scene test = SceneManager.GetSceneByName(_sceneName);
            Debug.Log(test.name == _sceneName + " " + _sceneName + " " + test.name);
        }

    }

}