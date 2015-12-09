using UnityEngine;
using UnityEngine.EventSystems;

namespace Hourai.Console {

    /// <summary>
    /// UI Element to toggle the appearance of the GameConsole UI.
    /// </summary>
	public class ConsoleToggle : MonoBehaviour {

        /// <summary>
        /// The KeyCode for the key that toggles the appearance of the GameConsole UI.
        /// </summary>
		[SerializeField]
		private KeyCode _key = KeyCode.F5;

        /// <summary>
        /// GameObjects to activate and deactivate when toggling the GameConsole UI.
        /// </summary>
		[SerializeField]
		private GameObject[] _toggle;

        /// <summary>
        /// GameObject to select when GameConsole is set to show.
        /// </summary>
	    [SerializeField]
	    private GameObject select; 

		void Update() {
			if(!Input.GetKeyDown(_key))
				return;
			foreach(GameObject go in _toggle) {
				if(!go)
					continue;
				go.SetActive(!go.activeSelf);
			}
            EventSystem.current.SetSelectedGameObject(select);
		}
		
	}

}
