using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class CharacterData : ScriptableObject {

        [SerializeField]
        [Tooltip("The localization key used for the character's shortened name")]
        private string _shortNameKey;

        [SerializeField]
        [Tooltip("The localization key used for the character's full name.")]
        private string _fullNameKey;

        [SerializeField, Resource(typeof(Sprite))]
        private string[] _alternativePortraits;

        [SerializeField, Resource(typeof(GameObject))]
        [Tooltip("The prefab of the Character to spawn.")]
        private string _prefab;

        [SerializeField, Resource(typeof(Sprite))]
        [Tooltip("The icon used to represent the character.")]
        private string _icon;

        [SerializeField, Resource(typeof (SceneData))]
        [Tooltip("The Character's associated stage.")]
        private string _homeStage;

        [SerializeField, Tooltip(" Is the Character selectable from the character select screen?")]
        private bool _isSelectable;

        [SerializeField, Tooltip("Is the Character viewable in the character select screen?")]
        private bool _isVisible;

        private Resource<Sprite> _iconResource;
        private Resource<GameObject> _prefabResource;
        private Resource<Sprite>[] _portraitResources;
        private Resource<SceneData> _homeStageResource; 

        /// <summary>
        /// The short name of the character. Usually just their first name.
        /// </summary>
        public string ShortName {
            get { return _shortNameKey; }
        }

        /// <summary>
        /// The full name of the character.
        /// </summary>
        public string FullName {
            get { return _fullNameKey; }
        }

        /// <summary>
        /// Is the Character selectable from the character select screen?
        /// </summary>
        public bool IsSelectable {
            get { return _isSelectable && _isVisible; }
        }

        /// <summary>
        /// Is the Character viewable in the character select screen?
        /// </summary>
        public bool IsVisible {
            get { return _isVisible; }
        }

        public int AlternativeCount {
            get { return _alternativePortraits == null ? 0 : _alternativePortraits.Length; }
        }

        public Resource<Sprite> Icon {
            get { return _iconResource; }
        }

        public Resource<SceneData> HomeStage {
            get { return _homeStageResource;  }
        }

        public Resource<GameObject> Prefab {
            get { return _prefabResource; }
        } 

        public Resource<Sprite> GetPortrait(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return _portraitResources[alternativeChoice];
        }

        /// <summary>
        /// Unity Callback. Called when the asset instance is loaded into memory.
        /// </summary>
        void OnEnable() {
            if (_alternativePortraits == null)
                return;
            _iconResource = new Resource<Sprite>(_icon);
            _prefabResource = new Resource<GameObject>(_prefab);
            _portraitResources = new Resource<Sprite>[_alternativePortraits.Length];
            _homeStageResource = new Resource<SceneData>(_homeStage);
            for(var i = 0; i < _alternativePortraits.Length; i++)
                _portraitResources[i] = new Resource<Sprite>(_alternativePortraits[i]);
        }

    }

}