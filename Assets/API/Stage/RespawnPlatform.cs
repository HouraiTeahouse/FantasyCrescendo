using UnityEngine;
using System.Collections;

namespace Crescendo.API {
    
    
    public class RespawnPlatform : GensoBehaviour {

        private Character character;
        private Rigidbody physics;

        public Character Character {
            get { return character; }
            set {
                if (value == null)
                    return;
                value.IsInvincible = true;
                character = value;
                physics = value.GetComponent<Rigidbody>();
                if (physics != null)
                    physics.velocity = Vector3.zero;
            }
        }

        [SerializeField]
        private float _invicibilityTimer;

        [SerializeField]
        private float _platformTimer;

        private float timer = 0f;
        
	
	    // Update is called once per frame
	    void Update () {
	        if (character == null)
	            return;

	        timer += Util.dt;

            // TODO: Find better alternative to this hack
            if (timer > _platformTimer || (physics != null && physics.velocity.magnitude > 0.5f))
                Destroy(gameObject);

	    }

        void OnDestroy() {
            if (Character == null)
                return;

            Character.TemporaryInvincibility(_invicibilityTimer);
        }
    }
}