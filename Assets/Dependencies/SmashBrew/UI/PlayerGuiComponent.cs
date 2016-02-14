using UnityEngine;

namespace Hourai.SmashBrew.UI {
    
    public abstract class PlayerGuiComponent<T> : MonoBehaviour, IPlayerGUIComponent where T : Component {
        
        protected T Component { get; private set; }
        protected Character Character { get; private set; }

        public void SetPlayerData(Player data) {
            if (data == null || data.PlayerObject == null) {
                Component = null;
                Character = null;
                return;
            }
            Character = data.PlayerObject;
            Component = Character.GetComponentInChildren<T>();
        }

    }


}
