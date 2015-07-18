using UnityEngine;
using System.Collections;

namespace Genso.API {
    
    
    public class RespawnPlatform : MonoBehaviour {

        private Character character;

        public Character Character {
            get { return character; }
            set {
                if (value == null)
                    return;
                value.IsInvincible = true;
                character = value;
            }
        }

        public float InvincibilityTimer {
            get;
            set;
        }

        public float PlatformTimer {
            get; set;
        }

        private float timer = 0f;
        
	
	    // Update is called once per frame
	    void Update () {
	        if (character == null)
	            return;

	        timer += Util.dt;

            if(character.transform.position != transform.position || timer > PlatformTimer)
                Destroy(gameObject);

	    }

        void OnDestroy() {
            if (Character == null)
                return;

            Character.TemporaryInvincibility(InvincibilityTimer);
        }
    }
}