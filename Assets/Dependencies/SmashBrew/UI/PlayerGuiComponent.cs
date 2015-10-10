using UnityEngine;

namespace Hourai.SmashBrew.UI {
    
    public abstract class PlayerGuiComponent<T> : MonoBehaviour, IPlayerGUIComponent where T : CharacterComponent {
        
        protected T Component { get; private set; }
        protected Character Character { get; private set; }

        public void SetPlayerData(Player data) {
            if (data == null || data.SpawnedCharacter == null) {
                Component = null;
                Character = null;
                return;
            }
            Character = data.SpawnedCharacter;
            Component = Character.GetComponentInChildren<T>();
        }

    }


}