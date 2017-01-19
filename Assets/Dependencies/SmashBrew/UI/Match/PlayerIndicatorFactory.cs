using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning. </summary>
    public sealed class PlayerIndicatorFactory : MonoBehaviour {

        [SerializeField]
        PlayerIndicator _prefab;

        void Awake() {
            Log.Debug(PlayerManager.Instance.MatchPlayers.Count());
            foreach (Player player in PlayerManager.Instance.MatchPlayers) {
                PlayerIndicator indicator = Instantiate(_prefab);
                indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                indicator.name = "Player {0} Indicator".With(player.ID + 1);
                indicator.gameObject.SetActive(player.Type.IsActive);
                player.Changed += () => indicator.gameObject.SetActive(player.Type.IsActive);
            }
        }

    }

}
