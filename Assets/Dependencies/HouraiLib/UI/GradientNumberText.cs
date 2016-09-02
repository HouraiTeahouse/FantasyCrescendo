using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A NumberText element that changes the color the Text based on a the current number value and a defined
    /// Gradient. </summary>
    public class GradientNumberText : NumberText {

        [SerializeField]
        Gradient _gradient;

        [SerializeField]
        Range _range;

        /// <summary> The Color gradient used to determine the color of the text. </summary>
        public Gradient Gradient {
            get { return _gradient; }
            set { _gradient = value; }
        }

        protected override void UpdateText() {
            base.UpdateText();
            if (Text == null)
                return;
            if (Gradient == null) {
                Gradient = new Gradient();
                Gradient.SetKeys(new[] {new GradientColorKey(Text.color, 0)}, new GradientAlphaKey[] {});
            }
            Text.color = _gradient.Evaluate(_range.InverseLerp(Number));
        }

    }

}