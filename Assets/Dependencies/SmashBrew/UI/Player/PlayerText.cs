using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PlayerUIComponent that displays the Player's string representation on a] UI Text object. </summary>
    public sealed class PlayerText : PlayerUIComponent<Text> {

        [SerializeField]
        [Tooltip("Whether to use the short moniker or not.")]
        bool _short;

        /// <summary>
        ///     <see cref="PlayerUIComponent.PlayerChange" />
        /// </summary>
        protected override void PlayerChange() {
            base.PlayerChange();
            Component.text = Player != null ? Player.GetName(_short) : string.Empty;
        }

    }

}