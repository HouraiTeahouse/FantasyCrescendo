using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew.UI {
    
    public class DropdownUI : MonoBehaviour {

        [SerializeField]
        private GameObject _dropdownMenu = null;

        private Mediator _mediator;

        private void Start() {
            if (_dropdownMenu == null) {
                Debug.Log("Please set all game objects needed by the Dropdown component");
                return;
            }
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null) {
                Debug.Log("The dropdown component couldn't find the data manager");
                return;
            }

            _mediator = dataManager.Mediator;
        }

        public void SetDropdownActive(bool b) {
            _dropdownMenu.SetActive(b);
            _mediator.Publish(
                             new DataEvent.UserChangingOptions { IsUserChangingOptions = b });
        }

    }

}
