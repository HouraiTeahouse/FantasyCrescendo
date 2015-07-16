using UnityEngine;
using System.Collections;

namespace Genso.API {

    public class CharacterInput : CharacterComponent {

        public virtual bool GetJump() {
            //TODO: Abstract this to multiplayer setups
            return Input.GetButtonDown("Jump");
        }


    }

}