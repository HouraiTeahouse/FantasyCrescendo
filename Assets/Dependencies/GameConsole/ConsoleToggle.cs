using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.Console {

    /// <summary>
    /// UI Element to toggle the appearance of the GameConsole UI.
    /// </summary>
	public class ConsoleToggle : MonoBehaviour {

		[SerializeField, Tooltip("The KeyCode for the key that toggles the appearance of the GameConsole UI.")]
		private KeyCode _key = KeyCode.F5;

		[SerializeField, Tooltip("GameObjects to activate and deactivate when toggling the GameConsole UI.")]
		private GameObject[] _toggle;

	    [SerializeField, Tooltip("GameObject to selet when GameConsole is set to show")]
	    private GameObject select; 

        /// <summary>
        /// Unity callback. Called once every frame.
        /// </summary>
		void Update() {
			if(!Input.GetKeyDown(_key))
				return;
			foreach(GameObject go in _toggle) {
				if(!go)
					continue;
				go.SetActive(!go.activeSelf);
			}
            EventSystem eventSystem = EventSystem.current;
		    if(eventSystem)
                eventSystem.SetSelectedGameObject(select);
		}
		
	}

}
