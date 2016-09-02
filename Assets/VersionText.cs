using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HouraiTeahouse {

    public abstract class UITextBehaviour : UIBehaviour {

        string _displayedText;

        [SerializeField]
        string _format;

        [SerializeField]
        Text _text;

        /// <summary> The string value to display on the text. </summary>
        protected virtual string DisplayedText {
            get { return _displayedText; }
            set {
                _displayedText = value;
                if (_text)
                    _text.text = !_format.IsNullOrEmpty() ? _format.With(value) : value;
            }
        }

        /// <summary>
        ///     <para> See MonoBehaviour.Awake. </para>
        /// </summary>
        protected override void Awake() {
            base.Awake();
            if (!_text)
                _text = GetComponent<Text>();
        }

    }

    public sealed class VersionText : UITextBehaviour {

        protected override void Awake() {
            base.Awake();
            DisplayedText = Application.version;
        }

    }

}