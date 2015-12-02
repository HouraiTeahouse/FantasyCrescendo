using UnityEngine;
using UnityEngine.UI;

namespace Hourai.Console {

	[RequireComponent(typeof(InputField))]
	public class ConsoleCommandEntry : MonoBehaviour {

		private InputField _input;

		void Awake() {
			_input = GetComponent<InputField>();
		}

		void Update() {
			if(!isActiveAndEnabled || !Input.GetKeyDown(KeyCode.Return))
				return;
			string command = _input.TextComponent.text;
			_input.TextComponent.text = string.Empty;
			GameConsole.Execute(command);
		}

	}

}
