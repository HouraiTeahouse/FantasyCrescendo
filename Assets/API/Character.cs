using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Genso.API {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
    [RequireComponent(typeof(CharacterPhysics))]
	public class Character : GensoBehaviour
    {
        [Serializable]
        private class MovementData {

            public float WalkSpeed = 5f;
            public float RunSpeed = 10f;
            public float AirSpeed = 3f;
            public int MaxJumps = 2;
            public AnimationCurve JumpPower;

        }

        [Serializable]
        private class AnimationParameters {

            public AnimationBool Grounded = new AnimationBool("grounded");
            public AnimationFloat VerticalSpeed = new AnimationFloat("vertical speed");
            public AnimationFloat HorizontalSpeed = new AnimationFloat("horizontal speed");

            public void Initialize(Animator animator) {
                Grounded.Animator = animator;
                VerticalSpeed.Animator = animator;
                HorizontalSpeed.Animator = animator;
            }

        }

        [SerializeField]
        private MovementData movement;

        [SerializeField]
        private AnimationParameters animationInfo;
        
        private bool running;
        private Collider[] hurtboxes;

        protected float HorizontalSpeed
        {
            get
            {
                if (!Grounded)
                    return movement.AirSpeed;
                if (running)
                    return movement.RunSpeed;
                return movement.WalkSpeed;
            }
        }
        
        public Animator Animator { get; private set; }
        public CharacterController Controller { get; private set; }

        public int PlayerNumber { get; set; }
        public Transform RespawnPosition { get; set; }

        public float Height {
            get {
                return Controller ? Controller.height : 0;
            }
            protected set {
                if (Controller)
                    Controller.height = value;
            }
        }

        private bool _grounded;
        public bool Grounded {
            get { return _grounded; }
            protected set {
                _grounded = value;
                animationInfo.Grounded.Set(value);
                if (value)
                    JumpCount = 0;
            }
        }

        public int JumpCount { get; private set; }

        protected virtual void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            Controller = GetComponent<CharacterController>();

            List<Collider> tempHurtboxes = new List<Collider>();
            foreach (Collider collider in GetComponentsInChildren<Collider>()) {
                if((collider.gameObject.layer & GameSettings.HurtboxLayers) != 0)
                    Hurtbox.Register(this, collider);
            }
            hurtboxes = tempHurtboxes.ToArray();
			
			animationInfo.Initialize(Animator);
        }

        void Update() {
            if (Input.GetButtonDown("Jump"))
                Jump();
        }
        
        public virtual void Jump() {
            int maxJumps = movement.MaxJumps;
            Debug.Log(movement.MaxJumps);
            if (JumpCount < maxJumps) {
                float jumpPower = movement.JumpPower.Evaluate((maxJumps <= 0f) ? 0f : ((float)JumpCount / ((float)maxJumps - 1)));
                Debug.Log(jumpPower);
                //TODO: Reimplement jumping using Character Controller physics
                JumpCount++;
            }
        }

        protected virtual void OnDrawGizmos() {
            if (hurtboxes == null)
                return;

           GizmoUtil.DrawHitboxes(hurtboxes, HitboxType.Damageable, x => x.enabled);
        }

        protected virtual void OnTriggerEnter(Collider other) {
            GroundedCheck(other, true);
        }

        protected virtual void OnTriggerStay(Collider other) {
            GroundedCheck(other, true);
        }

        protected virtual void OnTriggerExit(Collider other) {
            GroundedCheck(other, false);
        }

        protected void GroundedCheck(Component other, bool value) {
            if (other == null)
                return;

            Grounded = (other.CompareTag("Platform")) ? value : !value;
        }

    }

}
