using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.Characters;

namespace HouraiTeahouse.FantasyCrescendo {

    public class StateDumpUI : MonoBehaviour {

        [SerializeField]
        Text _text;

        void Awake() {
            _text = GetComponent<Text>();
        }

        void Update() {
            if (_text == null)
                return;
            var manager = PlayerManager.Instance;
            if (manager == null) {
                _text.text = string.Empty;
                return;
            }
            var builder = new StringBuilder();
            foreach(var player in manager.MatchPlayers) {
                if (player.PlayerObject != null) {
                    var character = player.PlayerObject.GetComponentInChildren<Character>();
                    if (character != null) {
                        builder.AppendLine("P{0}: {1}".With(player.ID + 1, character.StateController.CurrentState.Name));
                    }
                } else {
                    builder.AppendLine("P{0}: NONE".With(player.ID + 1));
                }
            }
            _text.text = builder.ToString();
        }


    }

}

