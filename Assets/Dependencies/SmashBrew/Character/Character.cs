using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Hourai.Events;
using UnityConstants;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hourai.SmashBrew {

    /// <summary>
    /// General character class for handling the physics and animations of individual characters
    /// </summary>
    /// Author: James Liu
    /// Authored on 07/01/2015
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Character : HouraiBehaviour {

        private static readonly Type[] RequiredComponents;

        static Character() {
            var componentType = typeof(Component);
            var requiredComponentType = typeof(RequiredCharacterComponentAttribute);
            // Use reflection to find required Components for Characters and statuses
            // Enumerate all concrete Component types
            RequiredComponents = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                      assemblyType != null &&
                                      !assemblyType.IsAbstract &&
                                      componentType.IsAssignableFrom(assemblyType) &&
                                      assemblyType.IsDefined(requiredComponentType, true)
                                  select assemblyType).ToArray();
        }

        #region Public Properties

        public Mediator CharacterEvents {
            get; private set;
        }
        
        public ModifierList DamageDealt {
            get; private set;
        }

        public ModifierList KnockbackDealt {
            get; private set;
        }

        public int BoneCount {
            get { return _bones.Length; }
        }

        #endregion

        #region Runtime Variables
        private Transform[] _bones;
        #endregion

        #region Serialized Variables
        [SerializeField]
        private GameObject _rootBone;
        #endregion

        #region Public Events
        public event Action<Attack.Type, Attack.Direction, int> OnAttack;
        #endregion

        #region Required Components
        public CapsuleCollider MovementCollider { get; private set; }
        #endregion
        
        #region Public Action Methods
        public Transform GetBone(int boneIndex) {
            if (boneIndex < 0 || boneIndex >= BoneCount)
                return transform;
            return _bones[boneIndex];
        }
        #endregion

        #region Internal Methods
        internal float ModifyDamage(float baseDamage) {
            if (DamageDealt.Count <= 0)
                return baseDamage;
            return DamageDealt.Modifiy(baseDamage);
        }

        internal void Attack(Attack.Type type, Attack.Direction direction, int index) {
            if (OnAttack != null)
                OnAttack(type, direction, index);
        }
        #endregion

        #region Unity Callbacks
        protected override void Awake() {
            base.Awake();
            CharacterEvents = new Mediator();
            Reset();

            DamageDealt = new ModifierList();
            KnockbackDealt = new ModifierList();

            GameObject root = gameObject;

            if (_rootBone)
                root = _rootBone;

            _bones = root.GetComponentsInChildren<Transform>();

            // Initialize all animation behaviours
            BaseAnimationBehaviour.InitializeAll(Animator);
        }

        void Reset() {
            MovementCollider = GetComponent<CapsuleCollider>();
            MovementCollider.isTrigger = false;

            gameObject.tag = Tags.Player;
            gameObject.layer = Layers.Character;

            Rigidbody rb = Rigidbody;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.isKinematic = false;
            rb.useGravity = false;

            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            // Attach Required Components
            foreach (Type requriedType in RequiredComponents)
                if (gameObject.GetComponent(requriedType) == null)
                    gameObject.AddComponent(requriedType);
        }

        protected virtual void OnAnimatorMove() {
            //TODO: Merge Physics and Animation Movements here

            //_rigidbody.velocity = _animator.deltaPosition / Time.deltaTime;
        }
        #endregion
    }

}