using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary>
    /// A UI component that assigns a color to a Graphic according to a Player's color
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof (Graphic))]
    public sealed class PlayerUIColor : MonoBehaviour, IDataComponent<Player> {

        private enum AlphaBlend {
            // Applys the alpha change multiplicatively
            Multiplicative,
            // Applys the alpha change additively, by subtraction 
            Additive
        }

        [SerializeField, Tooltip("The target Graphic to apply the color to")]
        private Graphic _graphic;

        private Player _player;
        
        [SerializeField, Range(0f, 1f), Tooltip("The change in the color's saturation")]
        private float _saturation = 1f;

        [SerializeField, Range(0f, 1f), Tooltip("The change in the color's value")]
        private float _value = 1f;

        [SerializeField, Range(0f, 1f), Tooltip("The change in alpha")]
        private float _alpha = 1f;

        [SerializeField, Tooltip("The alpha blend stylej")]
        private AlphaBlend _alphaBlend;

        [SerializeField, Tooltip("The red channel color correction curve")]
        private AnimationCurve _red = AnimationCurve.Linear(0, 0, 1f, 1f);
        
        [SerializeField, Tooltip("The blue channel color correction curve")]
        private AnimationCurve _blue = AnimationCurve.Linear(0, 0, 1f, 1f);
        
        [SerializeField, Tooltip("The green channel color correction curve")]
        private AnimationCurve _green = AnimationCurve.Linear(0, 0, 1f, 1f);

        /// <summary>
        /// The Player's color after adjustment
        /// </summary>
        public Color AdjustedColor {
            get {
                Color rawColor = _player != null ? _player.Color : Color.clear;

                rawColor.r = _red.Evaluate(rawColor.r);
                rawColor.g = _green.Evaluate(rawColor.g);
                rawColor.b = _blue.Evaluate(rawColor.b);

                if (_alphaBlend == AlphaBlend.Multiplicative)
                    rawColor.a = _alpha*rawColor.a;
                else
                    rawColor.a = rawColor.a - (1 - _alpha);

                HSV hsv = rawColor;
                hsv.s *= _saturation;
                hsv.v *= _value;
                return hsv;
            }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            if(_graphic == null)
                _graphic = GetComponent<Graphic>();
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            if (_player != null)
                _player.OnChanged -= UpdateColor;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Unity Callback. Called once per frame in-game, and 100 times/sec in the Editor.
        /// </summary>
        void Update() {
            if(Application.isPlaying)
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
            _graphic.color = AdjustedColor;
        }
#endif

        public void SetData(Player data) {
            if (data == null)
                return;
            if (_player != null)
                _player.OnChanged += UpdateColor;
            _player = data;
            _player.OnChanged += UpdateColor;
            UpdateColor();
        }

        void UpdateColor() {
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
            if (_graphic) {
                _graphic.color = AdjustedColor;
            }
        }
    }
}
