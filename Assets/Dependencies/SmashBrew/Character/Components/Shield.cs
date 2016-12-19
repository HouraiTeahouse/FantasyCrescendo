using UnityEngine;
using UnityEngine.Rendering;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerShieldBreakEvent {

    }

    [DisallowMultipleComponent]
    public sealed class Shield : BaseBehaviour, IDamageable {

        //[SerializeField]
        //float _maxHP = 100f;

        //[SerializeField]
        //float _regenerationRate = 10f;

        //[SerializeField]
        //float _depletionRate = 25f;

        //[SerializeField]
        //float _resetHP = 30f;

        //[SerializeField]
        //float _shieldSize = 1.5f;

        //float _currentHP;

        Character _character;
        PlayerController _playerController;
        //TODO: properly implement

        [SerializeField]
        Material _shieldMaterial;

        GameObject _shieldObj;
        Transform _shieldTransform;

        public void Damage(object source, float damage) { }

        protected override void Awake() {
            base.Awake();

            _character = GetComponent<Character>();

            if (!_character)
                return;

            // Create Shield Object
            _shieldObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _shieldObj.name = "Shield";
            _shieldTransform = _shieldObj.transform;
            _shieldTransform.parent = _character.transform;
            _shieldTransform.localPosition = _character.MovementCollider.center;

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
            _character.Events.Publish(new PlayerShieldBreakEvent());
            //_currentHP = _resetHP;
        }

    }

}
