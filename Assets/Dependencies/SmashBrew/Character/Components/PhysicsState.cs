using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Physics State")]
    [RequireComponent(typeof(CharacterController))]
    public class PhysicsState : CharacterNetworkComponent {

        // Character Constrants
        [SerializeField]
        [Tooltip("How much the character weighs")]
        float _weight = 1.0f;

        [SerializeField]
        [Tooltip("How fast a charactter reaches their max fall speed, in seconds.")]
        float _gravity = 1.5f;

        // Character Variables 
        Vector2 _velocity;
        Vector2 _acceleration;

        [SyncVar]
        bool _grounded;

        [SyncVar]
        bool _isFastFalling;

        public float Weight {
            get { return _weight; }
        }

        public float Gravity {
            get { return _gravity; }
        }

        public Vector2 Velocity {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector2 Acceleration {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        public bool IsGrounded {
            get { 
                if (Velocity.y > 0)
                    return false;
                var center = Vector3.zero;
                var radius = 1f;
                if (CharacterController != null) {
                    center = CharacterController.center - Vector3.up * (CharacterController.height * 0.50f - CharacterController.radius * 0.5f);
                    radius = CharacterController.radius * 0.75f;
                }
                return Physics.OverlapSphere(transform.TransformPoint(center), 
                                             radius, Config.Tags.StageMask, 
                                             QueryTriggerInteraction.Ignore)
                                             .Any(col => !_ignoredColliders.Contains(col));
            }
        }

        HashSet<Collider> _ignoredColliders;

        public override void ResetState() {
            Velocity = Vector2.zero;
            Acceleration = Vector2.zero;
        }

        public CharacterController CharacterController { get; private set; }

        public bool IsFastFalling {
            get { return _isFastFalling; }
        }

        public void SetHorizontalVelocity(float speed) { _velocity.x = speed; }
        public void SetVerticalVelocity(float speed) { _velocity.y = speed; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() { 
            base.Awake();
            CharacterController = GetComponent<CharacterController>(); 
            _ignoredColliders = new HashSet<Collider>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (!hasAuthority)
                return;
            var grounded = IsGrounded;
            var acceleration = Acceleration + Vector2.down * Gravity;
            if (CharacterController.isGrounded)
                acceleration.y = 0;
            Velocity += acceleration * Time.deltaTime;
            CharacterController.Move(Velocity * Time.deltaTime);
            if (!grounded)
                return;
            var originalPos = transform.position;
            var collision = CharacterController.Move(Vector3.down * Config.Physics.GroundSnapDistance);
            if (collision == CollisionFlags.None)
                transform.position = originalPos;
        }

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        void LateUpdate() {
            transform.SetZ(0);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            context.IsGrounded = IsGrounded;
        }

        public void IgnoreCollider(Collider collider, bool state) {
            Physics.IgnoreCollision(CharacterController, collider, state);
            if (state)
                _ignoredColliders.Add(collider);
            else
                _ignoredColliders.Remove(collider);
        }

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos() {
            var center = Vector3.zero;
            var radius = 1f;
            if (CharacterController != null) {
                center = CharacterController.center - Vector3.up * (CharacterController.height * 0.5f - CharacterController.radius * 0.5f);
                radius = CharacterController.radius * 0.75f;
                var diff = Vector3.up * (CharacterController.height * 0.5f - CharacterController.radius);
                using (Gizmo.With(Color.red)) {
                    var rad =  CharacterController.radius * transform.lossyScale.Max();
                    Gizmos.DrawWireSphere(transform.TransformPoint(CharacterController.center + diff), rad);
                    Gizmos.DrawWireSphere(transform.TransformPoint(CharacterController.center - diff), rad);
                }
            }
            using (Gizmo.With(Color.blue)) {
                Gizmos.DrawWireSphere(transform.TransformPoint(center), radius);
            }
        }

    }

}

