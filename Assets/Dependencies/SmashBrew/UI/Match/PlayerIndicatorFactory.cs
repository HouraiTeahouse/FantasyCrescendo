using System;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning. </summary>
    public sealed class PlayerIndicatorFactory : MonoBehaviour {

        [SerializeField]
        PlayerIndicator _prefab;

        Action[] PlayerActions;

        void Awake() {
            PlayerActions = new Action[PlayerManager.Instance.MatchPlayers.Count];
            foreach (Player player in PlayerManager.Instance.MatchPlayers) {
                PlayerIndicator indicator = Instantiate(_prefab);
                indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                indicator.gameObject.SetActive(player.Type.IsActive);
                PlayerActions[player.ID] = () => indicator.gameObject.SetActive(player.Type.IsActive);
                player.Changed += PlayerActions[player.ID];
            }
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            foreach (Player player in PlayerManager.Instance.MatchPlayers) 
                player.Changed -= PlayerActions[player.ID];
        }

    }

}
