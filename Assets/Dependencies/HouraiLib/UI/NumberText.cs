using UnityEngine;
using UnityEngine.UI;

namespace Hourai {

    [ExecuteInEditMode]
    public class NumberText : MonoBehaviour {

        [SerializeField]
        private Text _text;

        [SerializeField]
        private float _number;

        public float Number {
            get { return _number; }
            set { _number = value; }
        }

        [SerializeField, Tooltip("The string format used to display")]
        private string _format;

        /// <summary>
        /// The Text UI object that is driven by this 
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

