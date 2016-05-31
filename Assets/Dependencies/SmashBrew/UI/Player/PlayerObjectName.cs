using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    public class PlayerObjectName : PlayerUIComponent {
        [SerializeField]
        private string _format;

        protected override void OnPlayerChange() {
            name = _format.With(Player.PlayerNumber + 1);
        }
    }
}
