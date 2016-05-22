using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public enum TouhouGame {
        HighlyResponsiveToPrayers = 0,
        StoryOfEasternWonderland,
        PhantasmagoriaOfDimDream,
        LotusLandStory,
        MysticSquare,
        EmbodimentOfScarletDevil,
        PerfectCherryBlossom,
        ImmaterialAndMissingPower,
        ImperishableNight,
        PhantasmagoriaOfFlowerView,
        ShootTheBullet,
        MountainOfFaith,
        ScarletWeatherRhapsody,
        SubterraneanAnimism,
        UndefinedFantasticObject,
        Hisoutensoku,
        DoubleSpoiler,
        FairyWars,
        TenDesires,
        HopelessMasquerade,
        ImpossibleSpellCard,
        UrbanLegendInLimbo,
        LegacyOfLunaticKingdom,
        Other
    }

    public enum TouhouStage {
        PlayableCharacter = 0,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Extra,
        Other
    }

    /// <summary>
    /// A ScriptableObject 
    /// </summary>
    /// <seealso cref="DataManager"/>
    /// <seealso cref="SceneData"/>
    [CreateAssetMenu(fileName = "New Character", menuName = "SmashBrew/Character Data")]
    [HelpURL("http://wiki.houraiteahouse.net/index.php/Dev:CharacterData")]
    public class CharacterData : ScriptableObject, IGameData {
        [Header("General Data")]
        [SerializeField]
        private TouhouGame _sourceGame = TouhouGame.Other;

        [SerializeField]
        private TouhouStage _sourceStage = TouhouStage.Other;

        [SerializeField, Resource(typeof (GameObject))]
        [Tooltip("The prefab of the Character to spawn.")]
        private string _prefab;

        [SerializeField, Resource(typeof (SceneData))]
        [Tooltip("The Character's associated stage.")]
        private string _homeStage;

        [SerializeField, Tooltip(" Is the Character selectable from the character select screen?")]
        private bool _isSelectable;

        [SerializeField, Tooltip("Is the Character viewable in the character select screen?")]
        private bool _isVisible;

        [Header("Localization Data")] 
        [SerializeField, Tooltip("The localization key used for the character's shortened name")]
        private string _shortNameKey;

        [SerializeField, Tooltip("The localization key used for the character's full name.")]
        private string _fullNameKey;

        [Header("2D Art Data")]
        [SerializeField, Resource(typeof (Sprite))]
        private string[] _portraits;
        private Resource<Sprite>[] _portraitResources;

        [SerializeField]
        private Color _backgroundColor = Color.white;

        [SerializeField]
        [Tooltip("The center of the crop for smaller cropped views")]
        private Vector2 _cropPositon;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The size of the crop. In normalized coordinates.")]
        private float _cropSize;

        [SerializeField, Resource(typeof (Sprite))]
        [Tooltip("The icon used to represent the character.")]
        private string _icon;

        [Header("Audio Data")]
        [SerializeField, Resource(typeof (AudioClip))]
        [Tooltip("The audio clip played for the Character's announer")]
        private string _announcerClip;

        [SerializeField, Resource(typeof (AudioClip))]
        [Tooltip("The theme played on the match results screen when the character wins")]
        private string _victoryTheme;

        /// <summary>
        /// The source game the character is from
        /// </summary>
        public TouhouGame SourceGame {
            get { return _sourceGame; }
        }

        /// <summary>
        /// The source stage the character is from
        /// </summary>
        public TouhouStage SourceStage {
            get { return _sourceStage; }
        }

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
        public Resource<Sprite> Icon { get; private set; }

        /// <summary>
        /// Get the resource for the character's home stage
        /// </summary>
        public Resource<SceneData> HomeStage { get; private set; }

        /// <summary>
        /// Gets the resource for the character's prefab
        /// </summary>
        public Resource<GameObject> Prefab { get; private set; }

        /// <summary>
        /// Gets the resource for the character's announcer clip 
        /// </summary>
        public Resource<AudioClip> Announcer { get; private set; }

        /// <summary>
        /// Gets the resource for the character's victory theme clip 
        /// </summary>
        public Resource<AudioClip> VictoryTheme { get; private set; }

        /// <summary>
        /// Gets the crop rect relative to a texture 
        /// </summary>
        /// <param name="texture">the texture to get the rect relative to</param>
        /// <returns>the crop rect</returns>
        public Rect CropRect(Texture texture) {
            if (!texture)
                return new Rect(0, 0, 1, 1);
            float extents = _cropSize / 2;
            return texture.UVToPixelRect(new Rect(_cropPositon.x - extents, _cropPositon.y - extents, _cropSize, _cropSize));
        }

        /// <summary>
        /// The color used in the character's select image
        /// </summary>
        public Color BackgroundColor {
            get { return _backgroundColor; }
        }

        /// <summary>
        /// Gets the resource for the sprite portrait for a certain pallete.
        /// </summary>
        /// <param name="pallete">the pallete color to choose</param>
        /// <exception cref="ArgumentException">thrown if <paramref name="pallete"/> is less than 0 or greater than <see cref="PalleteCount"/></exception>
        /// <returns></returns>
        public Resource<Sprite> GetPortrait(int pallete) {
            Check.Argument("pallete", Check.Range(pallete, PalleteCount));
            if (_portraits.Length != _portraitResources.Length)
                RegeneratePortraits();
            return _portraitResources[pallete];
        }

        /// <summary>
        /// Unity Callback. Called when the asset instance is loaded into memory.
        /// </summary>
        void OnEnable() {
            if (_portraits == null)
                return;
            Icon = new Resource<Sprite>(_icon);
            Prefab = new Resource<GameObject>(_prefab);
            HomeStage = new Resource<SceneData>(_homeStage);
            Announcer = new Resource<AudioClip>(_announcerClip);
            VictoryTheme = new Resource<AudioClip>(_victoryTheme);
            RegeneratePortraits();
        }

        /// <summary>
        /// Unity callback. Called when the asset instance is unloaded from memory.
        /// </summary>
        void OnDisable() {
            Unload();
        }

        public void Unload() {
            Icon.Unload();
            Prefab.Unload();
            HomeStage.Unload();
            VictoryTheme.Unload();
            foreach (Resource<Sprite> portrait in _portraitResources)
                portrait.Unload();
        }

        void RegeneratePortraits() {
            _portraitResources = new Resource<Sprite>[_portraits.Length];
            for (var i = 0; i < _portraits.Length; i++)
                _portraitResources[i] = new Resource<Sprite>(_portraits[i]);
        }
    }
}
