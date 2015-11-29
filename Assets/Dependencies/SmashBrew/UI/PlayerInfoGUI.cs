using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Hourai.SmashBrew.UI {

    public class PlayerInfoGUI : MonoBehaviour {

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
            // TODO: Create displays from prefabs instead of depending on a prebuilt one
            _displays = _displays.Where(display => display != null).ToList();
            IEnumerator<Player> players = SmashGame.ActivePlayers.GetEnumerator();
            foreach (var display in _displays) {
                if (!players.MoveNext()) {
                    display.SetActive(false);
                    continue;
                }

                if (playerIndicatorPrefab)
                {
                    PlayerIndicator indicator = Instantiate(playerIndicatorPrefab);
                    indicator.Target = players.Current;
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

