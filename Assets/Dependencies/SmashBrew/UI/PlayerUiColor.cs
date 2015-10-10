using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [ExecuteInEditMode]
    [RequireComponent(typeof (Graphic))]
    public class PlayerUiColor : MonoBehaviour, IPlayerGUIComponent {

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

        public virtual Color AdjustedColor {
            get {
                Color rawColor = _player != null ? _player.Color : Color.clear;

                rawColor.r = _red.Evaluate(rawColor.r);
                rawColor.g = _green.Evaluate(rawColor.g);
                rawColor.b = _blue.Evaluate(rawColor.b);
                rawColor.a = _alpha;

                return ChangeSaturation(rawColor, _saturation);
            }
        }

        private Color ChangeSaturation(Color input, float saturation) {
            const float Pr = .299f;
            const float Pg = .587f;
            const float Pb = .114f;

            float P = Mathf.Sqrt(input.r * input.r * Pr + input.g * input.g * Pg + input.b * input.b * Pb);

            input.r = P + (input.r - P) * saturation;
            input.g = P + (input.g - P) * saturation;
            input.b = P + (input.b - P) * saturation;

            return input;
        }

        void Awake() {
            _graphic = GetComponent<Graphic>();
        }

#if UNITY_EDITOR
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
            _player = data;
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
            if (_graphic)
                _graphic.color = AdjustedColor;
        }

    }

}