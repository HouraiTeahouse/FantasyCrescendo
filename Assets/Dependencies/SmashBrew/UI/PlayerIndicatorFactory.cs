using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew.UI {

    public class PlayerIndicatorFactory : MonoBehaviour {

        [SerializeField]
        private PlayerIndicator _prefab;

        private Mediator _eventManager;

        void Awake() {
            if (!_prefab) {
                Destroy(this);
                return;
            }
            _eventManager = GlobalEventManager.Instance;
            _eventManager.Subscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        }

        private void OnDestroy() {
            if (_eventManager != null)
                _eventManager.Unsubscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        }

        private void OnSpawnPlayer(SpawnPlayerEvent eventArgs) {
            if (eventArgs == null || eventArgs.Player == null)
                return;
            PlayerIndicator indicator = Instantiate(_prefab);
            indicator.Target = eventArgs.Player;
        }


    }

}