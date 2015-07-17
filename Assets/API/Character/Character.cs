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
	public class Character : GensoBehaviour
    {
        
        private bool running;
        private Collider[] hurtboxes;

        public int PlayerNumber { get; set; }
        public Transform RespawnPosition { get; set; }

        private bool _grounded;
        public bool Grounded {
            get { return _grounded; }
            protected set {
                _grounded = value;
                if (value)
                    JumpCount = 0;
            }
        }

        private Action OnJump;

        public int Height { get; set; }

        public int JumpCount { get; private set; }

        protected virtual void Awake()
        {
            List<Collider> tempHurtboxes = new List<Collider>();
            foreach (Collider collider in GetComponentsInChildren<Collider>()) {
                if((collider.gameObject.layer & GameSettings.HurtboxLayers) != 0)
                    Hurtbox.Register(this, collider);
            }
            hurtboxes = tempHurtboxes.ToArray();

            foreach (var component in GetComponentsInChildren<CharacterComponent>()) {
                if (component)
                    OnJump += component.OnJump;
            }

            CameraController.AddTarget(this);
        }
        void Update() {
            if (Input.GetButtonDown("Jump"))
                Jump();
        }

        public virtual void Jump() {
            OnJump.SafeInvoke();
        }

        protected virtual void OnDrawGizmos() {
            if (hurtboxes == null)
                return;

           GizmoUtil.DrawHitboxes(hurtboxes, HitboxType.Damageable, x => x.enabled);
        }

        protected void GroundedCheck(Component other, bool value) {
            if (other == null)
                return;

            Grounded = (other.CompareTag("Platform")) ? value : !value;
        }

    }

}
