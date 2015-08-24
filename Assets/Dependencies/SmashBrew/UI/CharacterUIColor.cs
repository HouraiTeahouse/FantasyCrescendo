using UnityEngine;
using UnityEngine.UI;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew.UI {

    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    public class CharacterUIColor : BetterBehaviour, ICharacterGUIComponent {

        private Graphic _graphic;

        public int PlayerNumber { get; set; }

        [Serialize, Show, fSlider(0f, 1f), Default(1f)]
        private float _saturation;

        [Serialize, Show, fSlider(0f, 1f), Default(1f)]
        private float _alpha;

        [Serialize, Show]
        private AnimationCurve _red = AnimationCurve.Linear(0, 0, 1f, 1f);

        [Serialize, Show]
        private AnimationCurve _blue = AnimationCurve.Linear(0, 0, 1f, 1f);

        [Serialize, Show]
        private AnimationCurve _green = AnimationCurve.Linear(0, 0, 1f, 1f);

        protected Color PlayerColor {
            get {
                int maxPlayers = SmashGame.MaxPlayers;
                if (PlayerNumber < 0 || PlayerNumber > maxPlayers)
                    return maxPlayers <= 0 ? Color.white : SmashGame.GetPlayerColor(0);
                return SmashGame.GetPlayerColor(PlayerNumber);
            }
        }

        public virtual Color AdjustedColor {
            get {
                Color rawColor = PlayerColor;

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

        public void SetCharacterData(CharacterMatchData data) {
            if (data == null)
                return;
            PlayerNumber = data.PlayerNumber;
            if (_graphic == null)
                _graphic = GetComponent<Graphic>();
            if (_graphic)
                _graphic.color = AdjustedColor;
        }

    }

}