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