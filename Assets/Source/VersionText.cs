using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.UI {

    public class VersionText : MonoBehaviour {

        [SerializeField]
        Text _text;

        [SerializeField]
        string _displayFormat = "v{}";

        void Awake() {
            if (_text == null)
                _text = GetComponent<Text>();
            Debug.Log(_text);
            if (_text == null)
                return;
            Debug.Log(Application.version);
            if (string.IsNullOrEmpty(_displayFormat))
                _text.text = Application.version;
            else
                _text.text = _displayFormat.With(Application.version);
        }

        void Reset() {
            _text = GetComponent<Text>();
        }

    }

}
