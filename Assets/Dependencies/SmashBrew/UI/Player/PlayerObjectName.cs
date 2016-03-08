using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerObjectName : PlayerUIComponent {

        [SerializeField] private string _format;

        protected override void OnPlayerChange() {
            name = string.Format(_format, Player.PlayerNumber + 1);
        }
    }

}
