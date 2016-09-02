// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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

    [CreateAssetMenu(fileName = "New Stage", menuName = "SmashBrew/Scene Data")]
    public class SceneData : BGMGroup, IGameData {
        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The icon used shown on menus to represent the scene.")]
        string _icon;

        [SerializeField]
        [Tooltip("Is this scene selectable on select screens?")]
        bool _isSelectable = true;

        [SerializeField]
        [Tooltip("Is this scene a stage?")]
        bool _isStage = true;

        [SerializeField]
        [Tooltip("Is this scene visible on select screens?")]
        bool _isVisible = true;

        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The image shown on menus to represent the scene.")]
        string _previewImage;

        /// <summary> The internal name of the scene. Must be in build settings. </summary>
        [SerializeField]
        [Scene]
        [Tooltip("The internal name of the scene. Must be in build settings.")]
        string _sceneName;

        /// <summary> The image shown on menus to represent the scene. </summary>
        public Resource<Sprite> PreviewImage { get; private set; }

        /// <summary> The icon used on menus to represent the scene. </summary>
        public Resource<Sprite> Icon { get; private set; }

        /// <summary> Is the scene described by this SceneData a stage? </summary>
        public bool IsStage {
            get { return _isStage; }
        }

        public bool IsSelectable {
            get { return _isSelectable; }
        }

        public bool IsVisible {
            get { return _isVisible; }
        }

        public void Unload() {
            PreviewImage.Unload();
            Icon.Unload();
        }

        /// <summary> Loads the scene described by the SceneData </summary>
        public void Load() {
            Mediator.Global.Publish(new LoadSceneEvent {
                LoadOperation = SceneManager.LoadSceneAsync(_sceneName),
                Scene = this
            });
        }

        /// <summary> Unity Callback. Called when ScriptableObject is loaded. </summary>
        protected override void OnEnable() {
            base.OnEnable();
            PreviewImage = new Resource<Sprite>(_previewImage);
            Icon = new Resource<Sprite>(_icon);
        }

        /// <summary> Unity callback. Called when ScriptableObject is unloaded. </summary>
        protected override void OnDisable() {
            base.OnDisable();
            Unload();
        }
    }
}
