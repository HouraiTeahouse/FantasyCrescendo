using UnityEngine;
using System;

namespace Genso.API {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Character : GensoBehaviour
    {
        [Serializable]
        private class MovementData {

            public float WalkSpeed = 5f;
            public float RunSpeed = 10f;
            public float AirSpeed = 3f;
            public int MaxJumps;
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
        private AnimationParameters animation;

        private bool grounded;
        private bool running;

        protected float HorizontalSpeed
        {
            get
            {
                if (!grounded)
                    return movement.AirSpeed;
                if (running)
                    return movement.RunSpeed;
                return movement.WalkSpeed;
            }
        }

        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public CapsuleCollider Collider { get; private set; }

        public int PlayerNumber { get; set; }
        public Transform RespawnPosition { get; set; }

        public float Height
        {
            get
            {
                return Collider ? Collider.height : 0;
            }
            protected set
            {
                if (Collider)
                    Collider.height = value;
            }
        }

        public int JumpCount { get; private set; }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponent<Animator>();
            Collider = GetComponent<CapsuleCollider>();
            animation.Initialize(Animator);

            foreach (Collider collider in GetComponentsInChildren<Collider>()) {
                if((collider.gameObject.layer & GameSettings.HurtboxLayers) != 0)
                    Hurtbox.Register(this, collider);
            }
        }

        protected virtual void Update() {
            
            animation.Grounded.Set(grounded);

        }

        public virtual void Jump() {
            int maxJumps = movement.MaxJumps;
            if (JumpCount < movement.MaxJumps) {
                AnimationCurve jumpPower = movement.JumpPower;
                if (maxJumps <= 0)
                    Rigidbody.AddForce(transform.up * jumpPower.Evaluate(0f));
                else
                    Rigidbody.AddForce(transform.up * jumpPower.Evaluate((float)JumpCount / ((float)maxJumps - 1)));
                JumpCount++;
            }
        }

    }

}
