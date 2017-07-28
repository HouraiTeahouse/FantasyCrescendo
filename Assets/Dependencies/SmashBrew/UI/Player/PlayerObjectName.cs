using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerObjectName : PlayerUIComponent {

        [SerializeField]
        string _format;

        protected override void PlayerChange() { name = _format.With(Player.ID + 1); }

    }

}