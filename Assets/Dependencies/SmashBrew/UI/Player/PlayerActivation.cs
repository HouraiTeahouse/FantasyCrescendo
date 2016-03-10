using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerActivation : PlayerUIComponent {

        [Serializable]
        private class ActivateTarget {

            private enum Mode {
                Assign,
                And,
                Or,
                Xor
            }

            [SerializeField]
            private Behaviour Behaviour;
            [SerializeField]
            private Mode EditMode;
            [SerializeField]
            private bool IncludeCharacterCheck;
            [SerializeField]
            private bool Invert;

            public void Edit(Player player) {

                if (!Behaviour)
                    return;
                bool playerActive = player != null && player.IsActive;
                if (IncludeCharacterCheck)
                    playerActive &= player.SelectedCharacter != null;
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
        private ActivateTarget[] _targets;

        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            foreach (var target in _targets)
                target.Edit(Player);
        }
    }

}
