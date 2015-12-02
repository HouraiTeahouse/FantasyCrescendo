using UnityEngine;

namesace Hourai.Console {

	public class ToggleOnKeypress : MonoBehaviour {

		[SerializeField]
		private KeyCode _key = KeyCode.F5;

		[SerializeField]
		private GameObject[] _toggle;

		void Update() {
			if(!Input.GetKeyDown(_key))
				return;
			foreach(var go in _toggle) {
				if(!go)
					continue;
				go.SetActive(!go.activeSelf);
			}
		}
		
	}

}
