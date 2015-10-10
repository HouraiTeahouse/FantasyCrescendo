using UnityEngine;
using UnityEngine.UI;

namespace Hourai {

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class NumberText : MonoBehaviour {

        [SerializeField]
        private float _number;

        public float Number {
            get { return _number; }
            set { _number = value; }
        }

        [SerializeField]
        private string _format;

        private Text _text;

        protected Text Text {
            get { return _text; }
        }

        protected virtual void Update() {
            if (_text == null)
                _text = GetComponent<Text>();

            _text.text = string.IsNullOrEmpty(_format) ? _number.ToString() : _number.ToString(_format);
        }
    }
}

