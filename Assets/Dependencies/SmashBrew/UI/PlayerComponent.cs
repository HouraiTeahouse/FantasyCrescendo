using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {
    
    public abstract class PlayerComponent<T> : UIBehaviour, IDataComponent<Player> where T : Component {
        
        protected T Component { get; private set; }
        protected Character Character { get; private set; }

        public void SetData(Player data) {
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
