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
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> A UI component that assigns a color to a Graphic according to a Player's color </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    public sealed class PlayerUIColor : CharacterUIComponent<Graphic> {
        enum AlphaBlend {
            // Applys the alpha change multiplicatively
            Multiplicative,
            // Applys the alpha change additively, by subtraction 
            Additive
        }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The change in alpha")]
        float _alpha = 1f;

        [SerializeField]
        [Tooltip("The alpha blend stylej")]
        AlphaBlend _alphaBlend;

        [SerializeField]
        [Tooltip("The blue channel color correction curve")]
        AnimationCurve _blue = AnimationCurve.Linear(0, 0, 1f, 1f);

        [SerializeField]
        [Tooltip("The green channel color correction curve")]
        AnimationCurve _green = AnimationCurve.Linear(0, 0, 1f, 1f);

        [SerializeField]
        [Tooltip("The red channel color correction curve")]
        AnimationCurve _red = AnimationCurve.Linear(0, 0, 1f, 1f);

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The change in the color's saturation")]
        float _saturation = 1f;

        [SerializeField]
        bool _useCharacterColor;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The change in the color's value")]
        float _value = 1f;

        /// <summary> The Player's color after adjustment </summary>
        public Color AdjustedColor {
            get {
                Color rawColor = Player != null ? Player.Color : Color.white;

                if (_useCharacterColor && Character)
                    rawColor = Character.BackgroundColor;

                rawColor.r = _red.Evaluate(rawColor.r);
                rawColor.g = _green.Evaluate(rawColor.g);
                rawColor.b = _blue.Evaluate(rawColor.b);

                if (_alphaBlend == AlphaBlend.Multiplicative)
                    rawColor.a = _alpha * rawColor.a;
                else
                    rawColor.a = rawColor.a - (1 - _alpha);

                HSV hsv = rawColor;
                hsv.s *= _saturation;
                hsv.v *= _value;
                return hsv;
            }
        }

#if UNITY_EDITOR
        /// <summary> Unity Callback. Called once per frame in-game, and 100 times/sec in the Editor. </summary>
        void Update() {
            if (Application.isPlaying)
                if (Component == null)
                    Component = GetComponent<Graphic>();
            if (Component != null)
                Component.color = AdjustedColor;
        }
#endif

        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            if (Component)
                Component.color = AdjustedColor;
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component)
                Component.color = AdjustedColor;
        }
    }
}
