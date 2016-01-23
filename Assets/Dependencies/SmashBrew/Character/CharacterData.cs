using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    [CreateAssetMenu(fileName = "New Character", menuName = "SmashBrew/Character Data")]
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:CharacterData")]
    public class CharacterData : ScriptableObject {

        [Header("General Data")]
        [SerializeField, Resource(typeof(GameObject))]
        [Tooltip("The prefab of the Character to spawn.")]
        private string _prefab;

        [SerializeField, Resource(typeof (SceneData))]
        [Tooltip("The Character's associated stage.")]
        private string _homeStage;

        [SerializeField, Tooltip(" Is the Character selectable from the character select screen?")]
        private bool _isSelectable;

        [SerializeField, Tooltip("Is the Character viewable in the character select screen?")]
        private bool _isVisible;

        [SerializeField]
        [Tooltip("The localization key used for the character's shortened name")]
        [Header("Localization Data")]
        private string _shortNameKey;

        [SerializeField]
        [Tooltip("The localization key used for the character's full name.")]
        private string _fullNameKey;

        [Header("2D Art Data")]
        [SerializeField, Resource(typeof(Sprite))]
        private string[] _alternativePortraits;

        [SerializeField]
        [Tooltip("")]
        private Vector2 _cropPositon;

        [SerializeField]
        [Tooltip("")]
        private float _cropSize;

        [SerializeField, Resource(typeof(Sprite))]
        [Tooltip("The icon used to represent the character.")]
        private string _icon;

        [Header("Audio Data")]
        [SerializeField, Resource(typeof (AudioClip))]
        [Tooltip("The audio clip played for the Character's announer")]
        private string _announcerClip;

        [SerializeField, Resource(typeof (AudioClip))]
        [Tooltip("The theme played on the match results screen when the character wins")]
        private string _victoryTheme;

        private Resource<Sprite> _iconResource;
        private Resource<GameObject> _prefabResource;
        private Resource<Sprite>[] _portraitResources;
        private Resource<SceneData> _homeStageResource;
        private Resource<AudioClip> _announcerResource; 
        private Resource<AudioClip> _victoryThemeResource;

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

        public Resource<AudioClip> Announcer {
            get { return _announcerResource; }
        } 

        public Resource<AudioClip> VictoryTheme {
            get { return _victoryThemeResource; }
        }

        public Rect CropRect(Texture texture) {
            return new Rect(_cropPositon.x, _cropPositon.y, _cropSize, _cropSize * (float) texture.width / (float) texture.height);
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
            _victoryThemeResource = new Resource<AudioClip>(_victoryTheme);
            for(var i = 0; i < _alternativePortraits.Length; i++)
                _portraitResources[i] = new Resource<Sprite>(_alternativePortraits[i]);
        }

    }

}