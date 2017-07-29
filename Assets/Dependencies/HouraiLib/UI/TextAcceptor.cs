using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    public interface ITextAcceptor {

        int Priority { get; }

        void SetText(string text);

    }

    class UITextAcceptor : ITextAcceptor {

        readonly Text _text;

        public int Priority { get; private set; }

        internal UITextAcceptor(Text text) {
            _text = Argument.NotNull(text);
        }

        public void SetText(string text) {
            if (_text != null)
                _text.text = text;
        }

    }

    class TMPTextAcceptor : ITextAcceptor {

        readonly TMP_Text _text;

        public int Priority { get; private set; }

        internal TMPTextAcceptor(TMP_Text text) {
            _text = Argument.NotNull(text);
        }

        public void SetText(string text) {
            if (_text != null)
                _text.text = text;
        }

    }

    public static class ITextAcceptorExtensions {

        public static ITextAcceptor SetUIText(this GameObject gameObject, string text) {
            var acceptors = Argument.NotNull(gameObject).GetComponentsInChildren<ITextAcceptor>();
            if (acceptors.Any()) {
                var acceptor = acceptors.OrderByDescending(s => s.Priority).First();
                acceptor.SetText(text);
                return acceptor;
            }
            var uiText = gameObject.GetComponentInChildren<Text>();
            if (uiText != null) {
                var acceptor = new UITextAcceptor(uiText);
                acceptor.SetText(text);
                return acceptor;
            }
            var tmpText = gameObject.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) {
                var acceptor = new TMPTextAcceptor(tmpText);
                acceptor.SetText(text);
                return acceptor;
            }
            return null;
        }

    }


}