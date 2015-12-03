using UnityEngine;
using UnityEngine.UI;

namespace Hourai.Console {

	[RequireComponent(typeof(InputField))]
	public class ConsoleCommandEntry : MonoBehaviour {

		[SerializeField]
		private KeyCode _key = KeyCode.Return;

		private InputField _input;

		void Awake() {
			_input = GetComponent<InputField>();
		}

		void Update() {
			if(!isActiveAndEnabled || !Input.GetKeyDown(_key))
				return;
			string command = _input.textComponent.text;
			_input.textComponent.text = string.Empty;
			GameConsole.Execute(command);
		}

	}

}
