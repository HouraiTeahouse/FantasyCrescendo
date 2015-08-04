using UnityEngine;
using System.Collections;

namespace Crescendo.API {
    
    public class FallThrough : TriggerStageElement
    {

        void OnCollisionEnter(Collision col) {

            Debug.Log(col.collider);
            if (!col.collider.CompareTag(Game.PlayerTag))
                return;

            Character character = col.gameObject.GetComponentInParent<Character>();
            if (character == null)
                return;

            Debug.Log(character.InputSource.Crouch);

            if(character.InputSource.Crouch)
                ChangeIgnore(col.collider, true);

        }

    }

}