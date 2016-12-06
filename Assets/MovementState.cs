using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public class MovementState : NetworkBehaviour {

        public enum CharacterFacingMode {
            Rotation, Scale
        }

        [SerializeField]
        CharacterFacingMode _characterFacingMode;

        [SyncVar(hook = "OnChangeDirection"), SerializeField, ReadOnly]
        bool _direction;

        public bool Direction {
            get { return _direction; }
        }

        public CharacterFacingMode FacingMode {
            get { return _characterFacingMode; }
        }

        void Start() {
            OnChangeDirection(_direction);
        }

        void Update() {
            if (!isLocalPlayer)
                return;
            if (!Direction && Input.GetKey(KeyCode.LeftArrow))
                CmdSetDirection(true);
            if(Direction && Input.GetKey(KeyCode.RightArrow))
                CmdSetDirection(false);
        }

        [Command]
        void CmdSetDirection(bool direction) {
            Log.Info("SEND {1} {0}".With(direction, playerControllerId));
            _direction = direction;
        }

        void OnChangeDirection(bool direction) {
            Log.Info("RECIEVE {1} {0}".With(direction, playerControllerId));
            _direction = direction;
            if (FacingMode == CharacterFacingMode.Rotation) {
                var euler = transform.localEulerAngles;
                euler.y = direction ? 0 : 180;
                transform.localEulerAngles = euler;
            } else {
                var scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (direction ? -1 : 1);
                transform.localScale = scale;
            }
        }

    }

}

