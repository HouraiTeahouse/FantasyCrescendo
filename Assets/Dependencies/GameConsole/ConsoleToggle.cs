using UnityEngine;
using UnityEngine.EventSystems;

namespace Hourai.Console {

	public class ConsoleToggle : MonoBehaviour {

		[SerializeField]
		private KeyCode _key = KeyCode.F5;

		[SerializeField]
		private GameObject[] _toggle;

	    [SerializeField]
	    private GameObject select; 

		void Update() {
			if(!Input.GetKeyDown(_key))
				return;
			foreach(var go in _toggle) {
				if(!go)
					continue;
				go.SetActive(!go.activeSelf);
			}
            EventSystem.current.SetSelectedGameObject(select);
		}
		
	}

}
