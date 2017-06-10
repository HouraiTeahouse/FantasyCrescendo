using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HouraiTeahouse.UI {

    public class VersionText : MonoBehaviour {

        [SerializeField]
        Text _text;

        [SerializeField]
        TMP_Text _textMesh;

        [SerializeField]
        string _displayFormat = "v{0}";

        void Awake() {
            if (_text == null)
                _text = GetComponent<Text>();
            if (_textMesh == null)
                _textMesh = GetComponent<TMP_Text>();
            if (_text == null && _textMesh == null)
                return;
            if (string.IsNullOrEmpty(_displayFormat))
                SetText(Application.version);
            else
                SetText(_displayFormat.With(Application.version));
        }

        void SetText(string text) {
            if (_text != null)
                _text.text = text;
            if (_textMesh != null)
                _textMesh.text = text;
        }

        void Reset() {
            _text = GetComponent<Text>();
        }

    }

}
