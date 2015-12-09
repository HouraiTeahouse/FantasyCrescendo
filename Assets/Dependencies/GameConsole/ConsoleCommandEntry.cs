using UnityEngine;
using UnityEngine.UI;

namespace Hourai.Console {

    /// <summary>
    /// Turns a Text UI object into a entry line for GameConsole.
    /// Automatically reads, displays, and enters keyboard entered commands into the console.
    /// REQUIRED COMPONENT: UnityEngine.UI.Text
    /// </summary>
	[RequireComponent(typeof(Text))]
	public class ConsoleCommandEntry : MonoBehaviour {

		private Text _text;

        /// <summary>
        /// Unity Callback. Called on instantiation.
        /// </summary>
		void Awake() {
			_text = GetComponent<Text>();
		}

        /// <summary>
        /// Unity Callback. Called once per frame.
        /// </summary>
		void Update() {
            // Do nothing if not enabled.
			if(!isActiveAndEnabled)
				return;

            // Enter and execute command if Return/Enter is pressed
		    string input = Input.inputString;
		    if (input.Contains("\n") || input.Contains("\r")) {
                GameConsole.Execute(_text.text);
                _text.text = string.Empty;
            }
            // Remove the most recent character if Backspace is pressed
		    else if (input.Contains("\b")) {
                    string currentInput = _text.text;
                    if (string.IsNullOrEmpty(currentInput))
                        return;
                    _text.text = currentInput.Remove(currentInput.Length - 1);
		    } 
            // Otherwise extend the current command
            else {
                    _text.text += input; 
		    }
		}

	}

}
