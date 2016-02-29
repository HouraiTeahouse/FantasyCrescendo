using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary>
    /// A PlayerUIComponent that displays the Player's string representation on a]
    /// UI Text object.
    /// </summary>
    public sealed class PlayerText : PlayerUIComponent<Text> {

        /// <summary>
        /// <see cref="PlayerUIComponent{T}.OnPlayerChange"/>
        /// </summary>
        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            if (Player == null)
                Component.text = string.Empty;
            else
                Component.text = Player.ToString();
        }

    }

}
