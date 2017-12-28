using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

  public sealed class CharacterShield : MonoBehaviour {

    //// Character Constrants
    //[Header("Constants")]
    //public float ShieldSize = 1f;

    //[Tooltip("Maximum shield health for the character.")]
    //public float MaxShieldHealth = 100;

    //[Tooltip("How much shield health is lost over time.")]
    //public float DepletionRate = 15;

    //[Tooltip("How fast shield health replenishes when not in use.")]
    //public int RegenRate = 2;

    //public int RecoveryCooldown = 30;

    //[Tooltip("How much health the shield resets to after being broken.")]
    //public float ResetHealth = 30f;

    //public GameObject ShieldPrefab;
    //public Transform TargetBone;
    //public Material ShieldMaterial;

    //GameObject _shieldObj;
    //Transform _shieldTransform;
    //Hitbox _shieldHitbox;
    ////HashSet<CharacterState> validStates;
    ////float _lastShieldExitTime = float.NegativeInfinity;

    //public void Initalize(PlayerConfig config) {
      //base.Awake();
      //// Create Shield Object
      //_shieldObj = Instantiate(ShieldPrefab);
      //_shieldObj.name = "Shield";
      //_shieldTransform = _shieldObj.transform;
      //_shieldTransform.parent = transform;
      //_shieldTransform.localPosition = GetComponent<CharacterController>().center;

      //// Setup Collider
      //foreach (var collider in _shieldObj.GetComponentsInChildren<Collider>()) {
          //collider.isTrigger = true;
      //}

      //// Setup Renderers
      //foreach (var render in _shieldObj.GetComponentInChildren<Renderer>()) {
        //render.sharedMaterial = _shieldMaterial;
        //render.shadowCastingMode = ShadowCastingMode.Off;
        //render.receiveShadows = false;
        //render.reflectionProbeUsage = ReflectionProbeUsage.Off;
        //render.lightProbeUsage = LightProbeUsage.Off;
      //}

      //foreach (var hitbox in _shieldObj.GetComponentInChildren<Hitbox>()) {
        //_shieldHitbox.CurrentType = HitboxType.Shield;
      //}

      //SetShieldColor(Color.grey);
      //_shieldObj.SetActive(false);

      ////if (Character == null)
          ////return;
      ////var controller = Character.StateController;
      ////var states = Character.States.Shield;
      ////validStates = new HashSet<CharacterState>(new [] { states.On, states.Main, states.Perfect });
      //////TODO(james7132): Synchronize this across the network
      ////controller.OnStateChange += (b, a) => {
          ////_shieldObj.SetActive(validStates.Contains(a));
          ////if (_shieldObj.activeInHierarchy)
              //////TODO(james7132): Weigh the benefits of using scaled vs unscaled time for this
              ////_lastShieldExitTime = Time.time;
      ////};
    //}

      //public override PlayerState Simulate(Player state, PlayerInput input) {
        ////var machineState = GetState(state.StateHash);
        ////var shieldActive = machineState != null && validStates.Contains(machineState);
        //var shieldActive = false;
        //if (shieldActive) {
            //state.ShieldHealth = Mathf.Max(0f, state.ShieldHealth - DepletionRate * deltaTime);
        //} else {
            //// TODO(james7132): Make this non-dependent on the actual component state
            //if (Time.time > _lastShieldExitTime + RecoveryCooldown) {
                //state.ShieldHealth = Mathf.Min(MaxShieldHealth, state.ShieldHealth + RegenRate * deltaTime);
            //}
        //}
      //}

      //public override void ApplyState(PlayerState state) {
          //if (_targetBone != null && _shieldObj.activeInHierarchy) {
              //_shieldHitbox.CurrentType = HitboxType.Shield;
              //_shieldHitbox.CurrentType = HitboxType.Shield;
              //_shieldTransform.localScale = Vector3.one * _shieldSize * (state.ShieldHealth/MaxShieldHealth);
              //_shieldTransform.localPosition = transform.InverseTransformPoint(_targetBone.position);
          //}
      //}

      //public override void ResetState(ref CharacterStateSummary state) {
          //state.ShieldHealth = MaxShieldHealth;
      //}

      //public override void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
          //context.ShieldHP = summary.ShieldHealth;
      //}

      //void SetShieldColor(Color color) {
          //var render = _shieldObj.GetComponent<MeshRenderer>();
          //color.a = _shieldMaterial.color.a;
          //color = color * 0.75f;
          //render.material.color = color;
          //render.material.SetColor("_RimColor", color);
      //}

      ///// <summary> /// Unity Callback. Called from Editor to validate settings.  /// </summary>
      //void OnValidate() { _resetHealth = Mathf.Clamp(_resetHealth, 0f, _maxShieldHealth); }

  }

}
