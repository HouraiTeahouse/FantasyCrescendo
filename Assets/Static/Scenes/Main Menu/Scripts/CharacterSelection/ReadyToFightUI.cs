using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew.UI {
    
    public class ReadyToFightUI : MonoBehaviour {

        private Mediator mediator;
        public GameObject readyToFightButton = null;

        private void Start() {
            if (readyToFightButton == null) {
                Debug.Log("Please set all game objects needed by the ReadyToFightUI component");
                return;
            }
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null)
                Debug.Log("The ReadyToFightUI component couldn't find the data manager");

            mediator = dataManager.mediator;
            mediator.Subscribe<DataEvent.ReadyToFight>(onReadyToFight);
        }

        public void onReadyToFight(DataEvent.ReadyToFight cmd) {
            readyToFightButton.SetActive(cmd.isReady);
        }

    }

}
