using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Shield State")]
    public sealed class ShieldState : NetworkBehaviour, ICharacterState {

        // Character Constrants
        [Header("Constants")]
        [SerializeField]
        [Tooltip("Maximum shield health for the character.")]
        float _maxHealth = 100;

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
        Material _shieldMaterial;

        // Character Variables 
        [Header("Variables")]
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("How much shield health the character currently has")]
        float _health;

        public float MaxHealth {
            get { return _maxHealth; }
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
            get { return _health; }
            set { _health = value; }
        }

        GameObject _shieldObj;
        Transform _shieldTransform;

        public void ResetState() { Health = MaxHealth; }

        void Awake() {
            // Create Shield Object
            _shieldObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _shieldObj.name = "Shield";
            _shieldTransform = _shieldObj.transform;
            _shieldTransform.parent = transform;
            _shieldTransform.localPosition = GetComponent<CharacterController>().center;

            // Setup Collider
            _shieldObj.GetComponent<Collider>().isTrigger = true;

            // Setup Renderer
            var render = _shieldObj.GetComponent<MeshRenderer>();
            render.sharedMaterial = _shieldMaterial;
            render.shadowCastingMode = ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.reflectionProbeUsage = ReflectionProbeUsage.Off;
            render.lightProbeUsage = LightProbeUsage.Off;

            // Make sure the Color of the shield matches the HumanPlayer
            var player = GetComponentInParent<PlayerController>();
            if (player != null && player.PlayerData != null) {
                Color shieldColor = player.PlayerData.Color;
                shieldColor.a = _shieldMaterial.color.a;
                render.material.color = shieldColor;
            }
            _shieldObj.SetActive(false);

            //_currentHP = _maxHP;
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
        void OnValidate() { _resetHealth = Mathf.Clamp(_resetHealth, 0f, _maxHealth); }

    }

}

