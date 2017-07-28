using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Console {

    /// <summary> Displays the most recent logs in the GameConsole on a Text object. REQUIRED COMPONENT: UnityEngine.UI.Text </summary>
    [RequireComponent(typeof(Text))]
    public class ConsoleDisplay : MonoBehaviour {

        [SerializeField]
        int _limit = 20;

        // The Text component used to render the console history
        Text _displayText;

        // Used to build the log string from the console's history
        StringBuilder _textBuilder;

        // Has the Console been updated since the log was last rendered?
        bool _updated;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            _textBuilder = new StringBuilder();
            _updated = false;
            GameConsole.OnConsoleUpdate += Redraw;
            _displayText = GetComponent<Text>();
        }

        /// <summary> Unity Callback. Called on object destruction. </summary>
        void OnDestroy() { GameConsole.OnConsoleUpdate -= Redraw; }

        /// <summary> Unity Callback. Called each time the Behaviour is enabled or the GameObject it is attached to is activated. </summary>
        void OnEnable() {
            if (_updated)
                Redraw();
            _updated = false;
        }

        /// <summary> Re-reads history and renders it to the Text component. </summary>
        void Redraw() {
            if (!isActiveAndEnabled) {
                _updated = true;
                return;
            }
            // Clears the current string
            _textBuilder.Length = 0;
            var lines = GameConsole.History.SelectMany(log => log.Split('\n'));
            foreach (string log in lines.Skip(lines.Count() - _limit))
                _textBuilder.AppendLine(log);
            _displayText.text = _textBuilder.ToString();
        }

    }

}