using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterJump : RestrictableCharacterComponent {

        private const float defaultGravity = 9.86f;
        private CharacterGravity _gravity;

        [SerializeField]
        private float[] _jumpHeights = {1.5f, 1.5f};

        [Serialize]
        [AnimVar(Filter = ParameterType.Trigger, AutoMatch = "Trigger")]
        private int _jumpTrigger;

        [DontSerialize, Hide]
        public int JumpCount { get; private set; }

        public event Action OnJump;

        public int MaxJumpCount {
            get { return _jumpHeights == null ? 0 : _jumpHeights.Length; }
        }

        #region Unity Callbacks
        protected override void Start() {
            base.Start();
            _gravity = GetComponentInChildren<CharacterGravity>();

            Character.OnGrounded += delegate {
                                        if (Character.IsGrounded)
                                            JumpCount = 0;
                                    };
            Restrictions += delegate {
                                return JumpCount < MaxJumpCount;
                            };
        }

        void Update() {
            if (InputSource != null && InputSource.Jump)
                Jump();
        }
        #endregion

        public void Jump() {
            if (Restricted)
                return;

            float g = _gravity == null ? _gravity.Gravity : defaultGravity;

            // Apply upward force to jump
            Vector3 temp = Character.Velocity;
            temp.y = Mathf.Sqrt(2*g*_jumpHeights[JumpCount]);
            Character.Velocity = temp;

            JumpCount++;

            // Trigger animation
            Animator.SetTrigger(_jumpTrigger);

            OnJump.SafeInvoke();
        }

    }

}