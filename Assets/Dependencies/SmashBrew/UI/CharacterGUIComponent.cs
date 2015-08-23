using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew.UI {
    
    public abstract class CharacterGUIComponent<T> : BetterBehaviour, ICharacterGUIComponent where T : CharacterComponent {

        [DontSerialize, Hide]
        protected T Component { get; private set; }

        [DontSerialize, Hide]
        protected Character Character { get; private set; }


        public void SetCharacterData(CharacterMatchData data) {
            if (data == null || data.SpawnedInstance == null) {
                Component = null;
                Character = null;
                return;
            }
            Character = data.SpawnedInstance;
            Component = Character.GetComponentInChildren<T>();
        }

    }


}