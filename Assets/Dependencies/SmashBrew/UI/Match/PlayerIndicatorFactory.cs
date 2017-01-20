using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning. </summary>
    public sealed class PlayerIndicatorFactory : MonoBehaviour {

        [SerializeField]
        PlayerIndicator _prefab;

        void Awake() {
            foreach (Player player in PlayerManager.Instance.MatchPlayers) {
                PlayerIndicator indicator = Instantiate(_prefab);
                indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                indicator.gameObject.SetActive(player.Type.IsActive);
                player.Changed += () => indicator.gameObject.SetActive(player.Type.IsActive);
            }
        }

    }

}
