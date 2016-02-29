using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// A ScriptableObject 
    /// </summary>
    /// <seealso cref="DataManager"/>
    /// <seealso cref="SceneData"/>
    [CreateAssetMenu(fileName = "New Character", menuName = "SmashBrew/Character Data")]
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:CharacterData")]
    public class CharacterData : ScriptableObject {

        [Header("General Data")]
        [SerializeField, Resource(typeof(GameObject))]
        [Tooltip("The prefab of the Character to spawn.")]
        private string _prefab;
        private Resource<GameObject> _prefabResource;

        [SerializeField, Resource(typeof (SceneData))]
        [Tooltip("The Character's associated stage.")]
        private string _homeStage;
        private Resource<SceneData> _homeStageResource;

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
        private string[] _portraits;
        private Resource<Sprite>[] _portraitResources;

        [SerializeField]
        [Tooltip("The center of the crop for smaller cropped views")]
        private Vector2 _cropPositon;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The size of the crop. In normalized coordinates.")]
        private float _cropSize;

        [SerializeField, Resource(typeof(Sprite))]
        [Tooltip("The icon used to represent the character.")]
        private string _icon;
        private Resource<Sprite> _iconResource;

        [Header("Audio Data")]
        [SerializeField, Resource(typeof (AudioClip))]
        [Tooltip("The audio clip played for the Character's announer")]
        private string _announcerClip;
        private Resource<AudioClip> _announcerResource; 

        [SerializeField, Resource(typeof (AudioClip))]
        [Tooltip("The theme played on the match results screen when the character wins")]
        private string _victoryTheme;
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

        /// <summary>
        /// Gets how many palletes 
        /// </summary>
        public int PalleteCount {
            get { return _portraits == null ? 0 : _portraits.Length; }
        }

        /// <summary>
        /// Gets the resource for the character's icon
        /// </summary>
        public Resource<Sprite> Icon {
            get { return _iconResource; }
        }

        /// <summary>
        /// Get the resource for the character's home stage
        /// </summary>
        public Resource<SceneData> HomeStage {
            get { return _homeStageResource;  }
        }

        /// <summary>
        /// Gets the resource for the character's prefab
        /// </summary>
        public Resource<GameObject> Prefab {
            get { return _prefabResource; }
        }

        /// <summary>
        /// Gets the resource for the character's announcer clip 
        /// </summary>
        public Resource<AudioClip> Announcer {
            get { return _announcerResource; }
        } 

        /// <summary>
        /// Gets the resource for the character's victory theme clip 
        /// </summary>
        public Resource<AudioClip> VictoryTheme {
            get { return _victoryThemeResource; }
        }

        /// <summary>
        /// Gets the crop rect relative to a texture 
        /// </summary>
        /// <param name="texture">the texture to get the rect relative to</param>
        /// <returns>the crop rect</returns>
        public Rect CropRect(Texture texture) {
            return new Rect(_cropPositon.x, _cropPositon.y, _cropSize, _cropSize * (float) texture.width / (float) texture.height);
        }

        /// <summary>
        /// Gets the resource for the sprite portrait for a certain pallete.
        /// </summary>
        /// <param name="pallete">the pallete color to choose</param>
        /// <exception cref="ArgumentException">thrown if <paramref name="pallete"/> is less than 0 or greater than <see cref="PalleteCount"/></exception>
        /// <returns></returns>
        public Resource<Sprite> GetPortrait(int pallete) {
            if (pallete < 0 || pallete >= PalleteCount)
                throw new ArgumentException();
            if(_portraits.Length != _portraitResources.Length) 
                RegeneratePortraits();
            return _portraitResources[pallete];
        }

        /// <summary>
        /// Unity Callback. Called when the asset instance is loaded into memory.
        /// </summary>
        void OnEnable() {
            if (_portraits == null)
                return;
            _iconResource = new Resource<Sprite>(_icon);
            _prefabResource = new Resource<GameObject>(_prefab);
            _homeStageResource = new Resource<SceneData>(_homeStage);
            _announcerResource = new Resource<AudioClip>(_announcerClip);
            _victoryThemeResource = new Resource<AudioClip>(_victoryTheme);
            RegeneratePortraits();
        }

        void RegeneratePortraits() {
            _portraitResources = new Resource<Sprite>[_portraits.Length];
            for (var i = 0; i < _portraits.Length; i++) 
                _portraitResources[i] = new Resource<Sprite>(_portraits[i]);
        }
    }
}
