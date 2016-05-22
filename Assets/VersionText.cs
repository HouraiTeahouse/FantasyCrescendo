using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HouraiTeahouse {

    public abstract class UITextBehaviour : UIBehaviour {
        
        [SerializeField]
        private Text _text;

        [SerializeField]
        private string _format;

        private string _displayedText;
        protected virtual string DisplayedText {
            get { return _displayedText; }
            set {
                _displayedText = value;
                if (!_text)
                    return;
                if (!string.IsNullOrEmpty(_format))
                    _text.text = string.Format(_format, value);
                else
                    _text.text = value;
            }
        }

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
