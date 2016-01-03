using UnityEngine;
using UnityEngine.EventSystems;

namespace Hourai.SmashBrew.UI {

    /// <summary>
    /// An ISubmitHandler for exiting the game.
    /// Attach to a button or other "Submitable" elements to use to quit the game.
    /// </summary>
    public class ExitGame : MonoBehaviour, ISubmitHandler {

        // ISubmitHandler implementation
        public void OnSubmit(BaseEventData eventData) {
            Debug.Log("Game Exited.");
            Application.Quit();
        }

    }

}