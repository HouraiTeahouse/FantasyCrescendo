using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Shield State")]
    public sealed class ShieldState : CharacterNetworkComponent, IDataComponent<Player> {

        // Character Constrants
        [Header("Constants")]
        [SerializeField]
        float _shieldSize = 1f;

        [SerializeField]
        [Tooltip("Maximum shield health for the character.")]
        float _maxShieldHealth = 100;

        [SerializeField]
        [Tooltip("How much shield health is lost over time.")]
        float _depletionRate = 15;

        [SerializeField]
        [Tooltip("How fast shield health replenishes when not in use.")]
        float _regenRate = 25;

        [SerializeField]
        [Tooltip("How much health the shield resets to after being broken.")]
        float _resetHealth = 30f;

        [SerializeField]
        Transform _targetBone;

        [SerializeField]
        Material _shieldMaterial;

        // Character Variables 
        [Header("Variables")]
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("How much shield health the character currently has")]
        float _currentShieldHealth;

        public float MaxHealth {
            get { return _maxShieldHealth; }
        }

        public float DepletionRate {
            get { return _depletionRate; }
        }

        public float RegenRate {
            get { return _regenRate; }
        }

        public float ResetHealth {
            get { return _resetHealth; }
        }

        public float Health {
            get { return _currentShieldHealth; }
            set { _currentShieldHealth = value; }
        }

        GameObject _shieldObj;
        Transform _shieldTransform;

        public override void ResetState() { Health = MaxHealth; }

        protected override void Awake() {
            base.Awake();
            // Create Shield Object
            _shieldObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _shieldObj.name = "Shield";
            _shieldTransform = _shieldObj.transform;
            _shieldTransform.parent = transform;
            _shieldTransform.localPosition = GetComponent<CharacterController>().center;

            // Setup Collider
            foreach (var collider in _shieldObj.GetComponentsInChildren<Collider>())
                collider.isTrigger = true;

            // Setup Renderer
            var render = _shieldObj.GetComponent<MeshRenderer>();
            render.sharedMaterial = _shieldMaterial;
            render.shadowCastingMode = ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.reflectionProbeUsage = ReflectionProbeUsage.Off;
            render.lightProbeUsage = LightProbeUsage.Off;

            SetShieldColor(Color.grey);
            _shieldObj.SetActive(false);

            _currentShieldHealth = _maxShieldHealth;

            if (Character == null)
                return;
            var controller = Character.StateController;
            var states = Character.States.Shield;
            var validStates = new HashSet<CharacterState>(new [] { states.On, states.Main, states.Perfect });
            controller.OnStateChange += (b, a) => {
                _shieldObj.SetActive(validStates.Contains(a));
            };
        }

        void Update() {
            if (_targetBone != null && _shieldObj.activeInHierarchy) {
                _shieldTransform.localScale = Vector3.one * _shieldSize * (Health/MaxHealth);
                _shieldTransform.localPosition = transform.InverseTransformPoint(_targetBone.position);
                Health = Mathf.Max(0f, Health - DepletionRate * Time.deltaTime);
            } else {
                Health = Mathf.Min(MaxHealth, Health + RegenRate * Time.deltaTime);
            }
        }

        void SetShieldColor(Color color) {
            var render = _shieldObj.GetComponent<MeshRenderer>();
            color.a = _shieldMaterial.color.a;
            color = color * 0.75f;
            render.material.color = color;
            render.material.SetColor("_RimColor", color);
        }

        void IDataComponent<Player>.SetData(Player player) {
            if (player != null)
                SetShieldColor(player.Color);
        }

        //void FixedUpdate() {
        //    bool active = InputSource.Shield && Character.IsGrounded && InputSource.Movement == Vector2.zero;

        //    _shieldObj.SetActive(active);
        //    _currentHP += (active ? -_depletionRate : _regenerationRate) * Time.fixedDeltaTime;

        //    if (_currentHP < 0)
        //        PlayerShieldBreakEvent();
        //    else if (_currentHP > _maxHP)
        //        _currentHP = _maxHP;

        //    _shieldTransform.localPosition = Character.MovementCollider.center;
        //    _shieldTransform.localScale = Vector3.one * _shieldSize * (_currentHP/_maxHP);
        //}

        void ShieldBreak() {
            //_currentHP = _resetHP;
        }

        /// <summary> /// Unity Callback. Called from Editor to validate settings.  /// </summary>
        void OnValidate() { _resetHealth = Mathf.Clamp(_resetHealth, 0f, _maxShieldHealth); }

    }

}

