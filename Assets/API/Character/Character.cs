using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace Crescendo.API {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
    [DisallowMultipleComponent]
    [DefineCategories("Animation")]
    [RequireComponent(typeof (Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Character : GensoBehaviour, IDamageable, IKnocckbackable {

        private ActionRestriction _canAttack;
        private ActionRestriction _canJump;
        private ActionRestriction _canMove;

        private CapsuleCollider triggerCollider;
        
        [DontSerialize, Hide]
        public int PlayerNumber { get; internal set; }

        public Color PlayerColor {
            get { return Game.GetPlayerColor(PlayerNumber); }
        }

        [DontSerialize, Hide]
        public ICharacterInput InputSource { get; set; }

        public bool IsGrounded {
            get { return _grounded; }
            set {
                if (IsGrounded == value)
                    return;
                _grounded = value;
                Animator.Bools(__grounded, value);
                OnGrounded.SafeInvoke();
            }
        }

        public bool IsInvincible {
            get { return _invinicible; }
            set {
                if (_invinicible == value)
                    return;

                if (value)
                    Debug.Log(name + " is now invincible.");
                else
                    Debug.Log(name + " is no longer invincible.");

                _invinicible = value;
            }
        }

        public bool IsDashing {
            get { return IsGrounded && _dashing; }
            set { _dashing = value; }
        }

        public bool IsFastFalling {
            get { return !IsGrounded && InputSource != null && InputSource.Crouch; }
        }

        public bool IsCrouching {
            get { return IsGrounded && InputSource != null && InputSource.Crouch; }
        }

        public bool CanMove {
            get { return _canMove; }
            set { _canMove.Value = value; }
        }

        public bool CanJump {
            get { return _canJump; }
            set { _canJump.Value = value; }
        }

        public bool CanAttack {
            get { return _canAttack; }
            set { _canAttack.Value = value; }
        }

        public float Height {
            get { return MoevmentCollider.height; }
        }

        public void Damage(float damage) {
            if (!IsInvincible)
                OnDamage.SafeInvoke(damage);
        }

        public void Knockback(float baseKnockback) {
            OnKnockback.SafeInvoke();
        }

        public void AddForce(Vector3 force) {
            Rigidbody.AddForce(force);
        }

        public void AddForce(float x, float y) {
            Rigidbody.AddForce(x, y, 0f);
        }

        public void AddForce(float x, float y, float z) {
            Rigidbody.AddForce(x, y, z);
        }

        public event Action OnJump;
        public event Action OnGrounded;
        public event Action<Vector2> OnMove;
        public event Action OnBlastZoneExit;
        public event Action<float> OnDamage;
        public event Action OnKnockback;
        public event Action<bool> OnAttack;

        public event Func<bool> MovementRestrictions {
            add { _canMove.Add(value); }
            remove { _canMove.Remove(value); }
        }

        public event Func<bool> JumpRestrictions {
            add { _canJump.Add(value); }
            remove { _canJump.Remove(value); }
        }

        public event Func<bool> AttackRestrictions {
            add { _canAttack.Add(value); }
            remove { _canAttack.Remove(value); }
        }

        public virtual void Move(Vector2 direction) {
            if (CanMove)
                OnMove.SafeInvoke(direction);
        }

        public virtual void Jump() {
            OnJump.SafeInvoke();
        }

        public void BlastZoneExit() {
            OnBlastZoneExit.SafeInvoke();
        }

        public void TemporaryInvincibility(float time) {
            StartCoroutine(TempInvincibility(time));
        }

        public void LockMovement(float time) {
            StartCoroutine(TempLockMovement(time));
        }

        public void Attack() {
            if (!CanAttack)
                return;
            Animator.SetTrigger(_attack);
            OnAttack.SafeInvoke(false);
        }

        public void Special() {
            if (!CanAttack)
                return;
            Animator.SetTrigger(_special);
            OnAttack.SafeInvoke(true);
        }

        private IEnumerator TempInvincibility(float duration) {
            IsInvincible = true;

            var t = 0f;
            while (t < duration) {
                yield return null;
                t += Util.dt;
            }
            IsInvincible = false;
        }

        // Coroutine that prevents input from effecting character movement for a given amount of time (but retains velocity)
        // **Properly locks movement on characters that are not aerial, but for some reason they will not play animations
        // until they are grounded and all movement input is released**
        private IEnumerator TempLockMovement(float time) {
            CanMove = false;
            yield return new WaitForSeconds(time);
            CanMove = true;
        }

        private class ActionRestriction {

            private List<Func<bool>> _restrictions;
            public bool Value = true;

            public ActionRestriction() {
                _restrictions = new List<Func<bool>>();
            }

            public bool Evaluate() {
                if (!Value)
                    return false;
                for (int i = 0; i < _restrictions.Count; i++) {
                    if (!_restrictions[i]())
                        return false;
                }
                return true;
            }

            public void Add(Func<bool> restriction) {
                if (_restrictions.Contains(restriction))
                    return;
                _restrictions.Add(restriction);
            }

            public void Remove(Func<bool> restriction) {
                _restrictions.Remove(restriction);
            }

            public static implicit operator bool(ActionRestriction restriction) {
                return restriction.Evaluate();
            }

        }

        [Serialize]
        [Category("Animation")]
        [AnimVar(ParameterType.Trigger)]
        private int _attack;

        [Serialize]
        [Category("Animation")]
        [AnimVar(ParameterType.Trigger)]
        private int _special;

        [Serialize]
        [Category("Animation")]
        [AnimVar(ParameterType.Float)]
        private int _horizontalInput;

        [Serialize]
        [Category("Animation")]
        [AnimVar(ParameterType.Float)]
        private int _verticalInput;

        [Serialize]
        [Category("Animation")]
        [AnimVar(ParameterType.Bool)]
        private int __grounded;

        #region Serialized Variables

        [SerializeField]
        private float triggerSizeRatio = 1.5f;

        [SerializeField]
        private string _internalName;

        public string InternalName {
            get { return _internalName; }
            set { _internalName = value; }
        }

        #endregion

        #region Required Components

        [DontSerialize, Hide]
        public CapsuleCollider MoevmentCollider { get; private set; }

        [DontSerialize, Hide]
        public Rigidbody Rigidbody { get; private set; }

        [DontSerialize, Hide]
        public Animator Animator { get; private set; }

        #endregion

        #region State Variables

        private bool _grounded;
        private bool _helpless;
        private bool _facing;
        private bool _dashing;
        private bool _invinicible;

        #endregion

        #region Physics Properties

        public Vector3 Velocity {
            get { return Rigidbody.velocity; }
            set { Rigidbody.velocity = value; }
        }

        public float Mass {
            get { return Rigidbody.mass; }
            set { Rigidbody.mass = value; }
        }

        #endregion

        #region Unity Callbacks
        protected virtual void Awake() {
            MoevmentCollider = GetComponent<CapsuleCollider>();
            MoevmentCollider.isTrigger = false;

            triggerCollider = gameObject.AddComponent<CapsuleCollider>();
            triggerCollider.isTrigger = true;

            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            Animator = GetComponent<Animator>();

            var animationBehaviors = Animator.GetBehaviours<CharacterAnimationBehaviour>();
            foreach (var stateBehaviour in animationBehaviors)
                stateBehaviour.SetCharacter(this);

            _canMove = new ActionRestriction();
            _canJump = new ActionRestriction();
            _canAttack = new ActionRestriction();

            AttachRequiredComponents();
        }

        protected virtual void OnEnable() {
            // TODO: Find a better place to put this
            CameraController.AddTarget(this);
        }

        protected virtual void OnDisable() {
            // TODO: Find a better place to put this
            CameraController.RemoveTarget(this);
        }
        
        protected virtual void Reset() {
            AttachRequiredComponents();
        }

        protected virtual void Update() {
            // Sync Trigger and Movement Colliders
            triggerCollider.center = MoevmentCollider.center;
            triggerCollider.direction = MoevmentCollider.direction;
            triggerCollider.height = MoevmentCollider.height*triggerSizeRatio;
            triggerCollider.radius = MoevmentCollider.radius*triggerSizeRatio;
            
            if (InputSource == null)
                return;

            Vector2 movement = InputSource.Movement;

            Animator.Floats(_horizontalInput, Mathf.Abs(movement.x));
            Animator.Floats(_verticalInput, movement.y);

            // Now checks CanMove
            if (movement != Vector2.zero && CanMove)
                Move(movement);

            // Now checks CanMove
            if (InputSource.Jump && CanMove)
                Jump();

            if (InputSource.Attack)
                Attack();
        }

        protected virtual void OnAnimatorMove() {

            //TODO: Merge Physics and Animation Movements here
            
            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }
        #endregion

        public void AttachRequiredComponents() {
            var componentTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                      assemblyType.IsA<Component>()&&
                                      !assemblyType.IsAbstract &&
                                      assemblyType.IsDefined(typeof(RequiredCharacterComponentAttribute), true)
                                  select assemblyType);

            foreach (var requriedType in componentTypes) {
                if (requriedType != null)
                    this.GetOrAddComponent(requriedType);
            }
        }
    }

}