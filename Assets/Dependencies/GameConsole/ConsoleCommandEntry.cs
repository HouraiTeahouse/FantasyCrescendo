using UnityEngine;
using UnityEngine.UI;

namespace Hourai.Console {

	[RequireComponent(typeof(Text))]
	public class ConsoleCommandEntry : MonoBehaviour {

		[SerializeField]
		private KeyCode _key = KeyCode.Return;

		private Text _text;

		void Awake() {
			_text = GetComponent<Text>();
		}

		void Update() {
			if(!isActiveAndEnabled)
				return;
		    string input = Input.inputString;
		    if (input.Contains("\n") || input.Contains("\r")) {
                GameConsole.Execute(_text.text);
                _text.text = string.Empty;
            }
		    else if (input.Contains("\b")) {
                    string currentInput = _text.text;
                    if (string.IsNullOrEmpty(currentInput))
                        return;
                    _text.text = currentInput.Remove(currentInput.Length - 1);
		    } else {
                    _text.text += input; 
		    }
		}

	}

}
