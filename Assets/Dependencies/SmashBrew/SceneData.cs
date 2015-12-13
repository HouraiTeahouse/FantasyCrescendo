using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hourai.SmashBrew {

    public class SceneData : BGMGroup {

        [SerializeField]
        private string _sceneName;

        [SerializeField]
        private bool _isStage = true;

        [SerializeField, Resource(typeof (Sprite))]
        private string _previewImage;

        [SerializeField, Resource(typeof (Sprite))]
        private string _icon;

        public Resource<Sprite> PreviewImage { get; private set; }

        public Resource<Sprite> Icon { get; private set; }

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