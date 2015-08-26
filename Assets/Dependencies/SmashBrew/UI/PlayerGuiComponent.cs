using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew.UI {
    
    public abstract class PlayerGuiComponent<T> : BetterBehaviour, IPlayerGUIComponent where T : CharacterComponent {

        [DontSerialize, Hide]
        protected T Component { get; private set; }

        [DontSerialize, Hide]
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