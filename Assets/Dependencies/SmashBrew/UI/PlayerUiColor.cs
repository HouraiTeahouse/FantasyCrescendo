using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [ExecuteInEditMode]
    [RequireComponent(typeof (Graphic))]
    public sealed class PlayerUiColor : MonoBehaviour, IPlayerGUIComponent {

        private Graphic _graphic;
        private Player _player;
        
        [SerializeField, Range(0f, 1f)]
        private float _saturation = 1f;
        
        [SerializeField, Range(0f, 1f)]
        private float _alpha = 1f;

        [SerializeField]
        private AnimationCurve _red = AnimationCurve.Linear(0, 0, 1f, 1f);
        
        [SerializeField]
        private AnimationCurve _blue = AnimationCurve.Linear(0, 0, 1f, 1f);
        
        [SerializeField]
        private AnimationCurve _green = AnimationCurve.Linear(0, 0, 1f, 1f);

        public Color AdjustedColor {
            get {
                Color rawColor = _player != null ? _player.Color : Color.clear;

                rawColor.r = _red.Evaluate(rawColor.r);
                rawColor.g = _green.Evaluate(rawColor.g);
                rawColor.b = _blue.Evaluate(rawColor.b);
                rawColor.a = _alpha;

                const float Pr = .299f;
                const float Pg = .587f;
                const float Pb = .114f;

                float P = Mathf.Sqrt(rawColor.r * rawColor.r * Pr + rawColor.g * rawColor.g * Pg + rawColor.b * rawColor.b * Pb);

                rawColor.r = P + (rawColor.r - P) * _saturation;
                rawColor.g = P + (rawColor.g - P) * _saturation;
                rawColor.b = P + (rawColor.b - P) * _saturation;

                return rawColor;
            }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
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

        public void SetPlayerData(Player data) {
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
            if (_graphic)
                _graphic.color = AdjustedColor;
        }
    }

}