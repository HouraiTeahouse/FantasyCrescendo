﻿using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public sealed class CharacterShield : RestrictableCharacterComponent, IDamageable {

        [SerializeField]
        private Material _shieldMaterial;

        [SerializeField]
        private float _maxHP = 100f;

        [SerializeField]
        private float _regenerationRate = 10f;

        [SerializeField]
        private float _depletionRate = 25f;

        [SerializeField]
        private float _resetHP = 30f;

        [SerializeField]
        private float _shieldSize = 1.5f;

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

            // Make sure the Color of the shield matches the HumanPlayer
            Color shieldColor = Character.Player.Color;
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
            if(OnShieldBreak != null)
                OnShieldBreak();
            _currentHP = _resetHP;
        }

        public void Damage(IDamager source) {
            
        }

    }

}