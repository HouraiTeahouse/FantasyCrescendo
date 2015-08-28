using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew.UI {

    public class PlayerInfoGUI : BaseBehaviour {

        [SerializeField]
        private PlayerIndicator playerIndicatorPrefab;

        [SerializeField]
        private List<GameObject> _displays;

        void Awake() {
            Match.OnMatchStart += OnMatchStart;
        }

        void OnDestroy() {
            Match.OnMatchStart -= OnMatchStart;
        }

        void OnMatchStart() {
            _displays = _displays.Where(display => display != null).ToList();
            IEnumerator<Player> players = Match.ActivePlayers.GetEnumerator();
            foreach (var display in _displays) {
                if (!players.MoveNext()) {
                    display.SetActiveIfNot(false);
                    continue;
                }

                if (playerIndicatorPrefab)
                {
                    PlayerIndicator indicator = playerIndicatorPrefab.InstantiateNew();
                    indicator.Target = players.Current.SpawnedCharacter;
                }

                display.SetActive(true);

                // Update the display with the Character information
                foreach (IPlayerGUIComponent component in display.GetComponentsInChildren<IPlayerGUIComponent>())
                    component.SetPlayerData(players.Current);

                display.name = "Player " + (players.Current.PlayerNumber + 1) + " Display";
            }
        }

        void OnSpawn(Player data) {
        }

    }

}

