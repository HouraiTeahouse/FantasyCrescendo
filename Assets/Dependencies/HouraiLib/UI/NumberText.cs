using UnityEngine;
using UnityEngine.UI;

namespace Hourai {

    /// <summary>
    /// Displays a number to a UnityEngine.UI.Text UI object.
    /// </summary>
    [ExecuteInEditMode]
    public class NumberText : MonoBehaviour {

        [SerializeField, Tooltip("The Text UI object driven by this script")]
        private Text _text;

        [SerializeField, Tooltip("The number to display using this script")]
        private float _number;

        /// <summary>
        /// The number to be displayed by the Text UI object.
        /// </summary>
        public float Number {
            get { return _number; }
            set { _number = value; }
        }

        [SerializeField, Tooltip("The string format used to display")]
        private string _format;

        /// <summary>
        /// The string format used to display the number.
        /// </summary>
        public string Format {
            get { return _format; }
            set { _format = value; }
        }

        /// <summary>
        /// The Text UI object that is driven by this script.
        /// </summary>
        protected Text Text {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            if (_text == null)
                _text = GetComponent<Text>();
        }

        /// <summary>
        /// Unity Callback. Called when the script is added in the Editor or the Reset menu option is selected.
        /// </summary>
        void Reset() {
            _text = GetComponent<Text>();
        }

        /// <summary>
        /// Unity Callback. Called each frame.
        /// </summary>
        protected virtual void Update() {
            if (_text == null)
                return;
            _text.text = string.IsNullOrEmpty(_format) ? _number.ToString() : _number.ToString(_format);
        }
    }
}

