using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public sealed class JumpState : NetworkBehaviour, ICharacterState {

        // Character Constrants
        [Header("Constants")]
        [SerializeField]
        [Tooltip("")]
        float[] _jumpPower;

        // Character Variables 
        [Header("Variables")]
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("How many jumps the character has remaining")]
        int _jumpCount;

        public int MaxJumpCount {
            get { return _jumpPower == null ? 0 : _jumpPower.Length; }
        }

        public int JumpCount {
            get { return _jumpCount; }
            private set { _jumpCount = value;  }
        }

        public float GetJumpPower(int jumpNum) {
            if (_jumpPower == null)
                throw new InvalidOperationException();
            Argument.InRange(jumpNum, 0, _jumpPower.Length);
            return _jumpPower[jumpNum];
        }

        public void ResetState() { JumpCount = MaxJumpCount; }

    }

}
