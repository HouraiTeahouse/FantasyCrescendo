using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// A PlayerUIComponent that displays the Player's string representation on a]
    /// UI Text object.
    /// </summary>
    public sealed class PlayerText : PlayerUIComponent<Text> {
        [SerializeField, Tooltip("Whether to use the short moniker or not.")] private bool _short;

        /// <summary>
        /// <see cref="PlayerUIComponent{T}.OnPlayerChange"/>
        /// </summary>
        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            if (Player == null)
                Component.text = string.Empty;
            else
                Component.text = Player.GetName(_short);
        }
    }
}