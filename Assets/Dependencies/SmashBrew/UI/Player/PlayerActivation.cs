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

using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerActivation : PlayerUIComponent {

        [Serializable]
        class ActivateTarget {
            enum Mode {
                Assign,
                And,
                Or,
                Xor
            }

            [SerializeField]
            Behaviour Behaviour;

            [SerializeField]
            Mode EditMode;

            [SerializeField]
            bool IncludeCharacterCheck;

            [SerializeField]
            bool Invert;

            public void Edit(Player player) {
                if (!Behaviour)
                    return;
                bool playerActive = player != null && player.Type.IsActive;
                if (IncludeCharacterCheck)
                    playerActive &= player.Selection.Character != null;
                if (Invert)
                    playerActive = !playerActive;
                switch (EditMode) {
                    case Mode.Assign:
                        Behaviour.enabled = playerActive;
                        break;
                    case Mode.And:
                        Behaviour.enabled &= playerActive;
                        break;
                    case Mode.Or:
                        Behaviour.enabled |= playerActive;
                        break;
                    case Mode.Xor:
                        Behaviour.enabled ^= playerActive;
                        break;
                }
            }
        }

        [SerializeField]
        ActivateTarget[] _targets;

        protected override void PlayerChange() {
            base.PlayerChange();
            foreach (ActivateTarget target in _targets)
                target.Edit(Player);
        }
    }
}
