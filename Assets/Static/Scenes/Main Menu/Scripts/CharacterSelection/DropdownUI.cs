using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew.UI {
    
    public class DropdownUI : MonoBehaviour {

        [SerializeField]
        private GameObject dropdownMenu = null;

        private Mediator mediator;

        private void Start() {
            if (dropdownMenu == null) {
                Debug.Log("Please set all game objects needed by the Dropdown component");
                return;
            }
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null)
                Debug.Log("The dropdown component couldn't find the data manager");

            mediator = dataManager.mediator;
        }

        public void setDropdownActive(bool b) {
            dropdownMenu.SetActive(b);
            mediator.Publish(
                             new DataEvent.UserChangingOptions { isUserChangingOptions = b });
        }

    }

}
