using System;
using UnityEngine;
using UnityEngine.Rendering;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public sealed class CharacterShield : RestrictableCharacterComponent, IDamageable {

        [SerializeField]
        private Material _shieldMaterial;

        [Serialize, Default(100f)]
        private float _maxHP;

        [Serialize, Default(10f)]
        private float _regenerationRate;

        [Serialize, Default(25f)]
        private float _depletionRate;

        [Serialize, Default(30f)]
        private float _resetHP;
        
        [Serialize, Default(1.5f)]
        private float _shieldSize;

        private float _currentHP;

        private GameObject _shieldObj;
        private Transform _shieldTransform;

        public event Action OnShieldBreak;

        protected override void Start() {
            base.Start();

            // No point in continuing if Character is null
            if (Character == null)
                Destroy(this);

            // Create Shield Object
            _shieldObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _shieldObj.name = "Shield";
            _shieldTransform = _shieldObj.transform;
            _shieldTransform.parent = Character.transform;
            _shieldTransform.localPosition = Character.MovementCollider.center;

            // Setup Collider
            _shieldObj.GetComponent<Collider>().isTrigger = true;

            // Setup Renderer
            var render = _shieldObj.GetComponent<MeshRenderer>();
            render.sharedMaterial = _shieldMaterial;
            render.shadowCastingMode = ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.reflectionProbeUsage = ReflectionProbeUsage.Off;
            render.useLightProbes = false;

            // Make sure the Color of the shield matches the Player
            Color shieldColor = Character.PlayerColor;
            shieldColor.a = _shieldMaterial.color.a;
            render.material.color = shieldColor;

            _currentHP = _maxHP;
        }

        void FixedUpdate() {
            if (InputSource == null)
                return;

            bool active = InputSource.Shield && Character.IsGrounded && InputSource.Movement == Vector2.zero;

            _shieldObj.SetActive(active);
            _currentHP += (active ? -_depletionRate : _regenerationRate) * Time.fixedDeltaTime;

            if (_currentHP < 0)
                ShieldBreak();
            else if (_currentHP > _maxHP)
                _currentHP = _maxHP;

            _shieldTransform.localPosition = Character.MovementCollider.center;
            _shieldTransform.localScale = Vector3.one * _shieldSize * (_currentHP/_maxHP);
        }

        void ShieldBreak() {
            OnShieldBreak.SafeInvoke();
        }

        public void Damage(IDamageSource source) {
            
        }

    }

}
