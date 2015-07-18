using System;
using System.Collections.Generic;
using UnityEngine;

namespace Genso.API {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
    [RequireComponent(typeof(CapsuleCollider))]
	public class Character : GensoBehaviour {

        private enum FacingMode { Rotation, Scale }

        private CapsuleCollider movementCollider;
        private CapsuleCollider triggerCollider;

        [SerializeField]
        private FacingMode _facingMode = FacingMode.Rotation;

        [SerializeField]
        private float triggerSizeRatio = 1.5f;

        private bool _facing;
        private bool running;
        private Collider[] hurtboxes;

        public int PlayerNumber { get; set; }
        public Transform RespawnPosition { get; set; }

        private bool _grounded;
        public bool Grounded {
            get { return _grounded; }
            set {
                bool changed = _grounded == value;
                _grounded = value;
                if(changed)
                    OnGrounded.SafeInvoke();
            }
        }

        /// <summary>
        /// The direction the character is currently facing.
        /// If set to true, the character faces the right.
        /// If set to false, the character faces the left.
        /// 
        /// The method in which the character is flipped depends on what the Facing Mode parameter is set to.
        /// </summary>
        public bool Facing {
            get { return _facing; }
            set {
                if (_facing != value) {
                    if (_facingMode == FacingMode.Rotation)
                        transform.Rotate(0f, 180f, 0f);
                    else {
                        Vector3 temp = transform.localScale;
                        temp.x *= -1;
                        transform.localScale = temp;
                    }
                }
                _facing = value;
            }
        }

        public event Action OnJump;
        public event Action OnGrounded;
        public event Action<Vector2> OnMove;

        public float Height {
            get { return movementCollider.height; }
        }  

        public int JumpCount { get; private set; }

        protected virtual void Awake() {
            FindHurtboxes();
            CameraController.AddTarget(this);

            movementCollider = GetComponent<CapsuleCollider>();
            movementCollider.isTrigger = false;

            triggerCollider = gameObject.AddComponent<CapsuleCollider>();
            triggerCollider.isTrigger = true;
        }

        void FindHurtboxes() {
            List<Collider> tempHurtboxes = new List<Collider>();
            foreach (Collider collider in GetComponentsInChildren<Collider>()) {
                if (!collider.CheckLayer(GameSettings.HurtboxLayers))
                    continue;
                Hurtbox.Register(this, collider);
                tempHurtboxes.Add(collider);
            }
            hurtboxes = tempHurtboxes.ToArray();
        }

        internal void AddCharacterComponent(CharacterComponent component) {
            if(component == null)
                throw new ArgumentNullException("component");

            OnJump += component.OnJump;
            OnGrounded += component.OnGrounded;
            OnMove += component.OnMove;
        }

        internal void RemoveCharacterComponent(CharacterComponent component) {
            if(component == null)
                throw new ArgumentNullException("component");

            OnJump -= component.OnJump;
            OnGrounded -= component.OnGrounded;
            OnMove += component.OnMove;
        }

        void Update() {
            // Sync Trigger and Movement Colliders
            triggerCollider.center = movementCollider.center;
            triggerCollider.direction = movementCollider.direction;
            triggerCollider.height = movementCollider.height * triggerSizeRatio;
            triggerCollider.radius = movementCollider.radius * triggerSizeRatio;
            
            if (Input.GetButtonDown("Jump"))
                Jump();
        }

        public virtual void Jump() {
            OnJump.SafeInvoke();
        }

        protected virtual void OnDrawGizmos() {
           FindHurtboxes();
           GizmoUtil.DrawHitboxes(hurtboxes, HitboxType.Damageable, x => x.enabled);
        }

    }

}
