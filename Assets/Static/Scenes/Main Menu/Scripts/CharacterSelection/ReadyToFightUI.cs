using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    
    public class ReadyToFightUI : MonoBehaviour {

        private Mediator _mediator;
        public GameObject ReadyToFightButton = null;

        void Start() {
            if (ReadyToFightButton == null) {
                Debug.Log("Please set all game objects needed by the ReadyToFightUI component");
                return;
            }
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null) {
                Debug.Log("The ReadyToFightUI component couldn't find the data manager");
                return;
            }

            _mediator = dataManager.Mediator;
            _mediator.Subscribe<DataEvent.ReadyToFight>(OnReadyToFight);
        }

        public void OnReadyToFight(DataEvent.ReadyToFight cmd) {
            ReadyToFightButton.SetActive(cmd.IsReady);
        }

    }

}
