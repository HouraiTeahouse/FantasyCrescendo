using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary>
    /// An ISubmitHandler for exiting the game.
    /// Attach to a button or other "Submitable" elements to use to quit the game.
    /// </summary>
    public sealed class ExitGame : SingleActionBehaviour {
        protected override void Action() {
            Application.Quit();
        }
    }

}
