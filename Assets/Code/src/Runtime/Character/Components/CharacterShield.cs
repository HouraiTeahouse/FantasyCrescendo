using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace HouraiTeahouse.FantasyCrescendo {

public sealed class CharacterShield : MonoBehaviour, IPlayerSimulation, IPlayerView {

  // Character Constrants
  [Header("Constants")]
  public float ShieldSize = 1f;

  [Tooltip("Maximum shield health for the character.")]
  public uint MaxShieldHealth = 10000;

  [Tooltip("How much shield health is lost over time.")]
  public uint DepletionRate = 25;

  [Tooltip("How fast shield health replenishes when not in use.")]
  public uint RegenRate = 200;

  public uint RecoveryCooldown = 120;

  [Tooltip("How much health the shield resets to after being broken.")]
  public float ResetHealth = 30f;

  public GameObject ShieldPrefab;
  public Transform TargetBone;

  GameObject _shieldObj;
  Transform _shieldTransform;
  Hitbox _shieldHitbox;
  Renderer[] _shieldRenderers;
  //HashSet<CharacterState> validStates;
  //float _lastShieldExitTime = float.NegativeInfinity;

  void Awake() {
    // NOTE: This is only to allow the shield components to be stripped before
    // charater initialization. Do not do this in other situations.

    // Create Shield Object
    _shieldObj = Instantiate(ShieldPrefab);
    _shieldObj.name = "Shield";
    _shieldTransform = _shieldObj.transform;
    _shieldTransform.parent = transform;
    _shieldRenderers = _shieldObj.GetComponentsInChildren<Renderer>();
    //_shieldTransform.localPosition = GetComponent<CharacterController>().center;
    _shieldObj.SetActive(false);
  }

  public Task Initialize(PlayerConfig config, bool isView) {
    SetShieldColor(Config.Get<VisualConfig>().GetPlayerColor(config.PlayerID));

    //if (Character == null)
        //return;
    //var controller = Character.StateController;
    //var states = Character.States.Shield;
    //validStates = new HashSet<CharacterState>(new [] { states.On, states.Main, states.Perfect });
    ////TODO(james7132): Synchronize this across the network
    //controller.OnStateChange += (b, a) => {
        //_shieldObj.SetActive(validStates.Contains(a));
        //if (_shieldObj.activeInHierarchy)
            ////TODO(james7132): Weigh the benefits of using scaled vs unscaled time for this
            //_lastShieldExitTime = Time.time;
    //};

    return Task.CompletedTask;
  }

  public void Presimulate(PlayerState state) {
    ApplyState(state);
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    //var machineState = GetState(state.StateHash);
    //var shieldActive = machineState != null && validStates.Contains(machineState);
    var wasActive = false;
    var shieldActive = false;
    if (shieldActive) {
      state.ShieldDamage = Math.Min(MaxShieldHealth, state.ShieldDamage + DepletionRate);
      if (state.ShieldDamage <= 0f) {
        //TODO(james7132): Do ShieldBreak check here.
      }
    } else {
      if (wasActive) {
        state.ShieldRecoveryCooldown = RecoveryCooldown;
      } else {
        state.ShieldRecoveryCooldown = (uint)Mathf.Max(0, state.ShieldRecoveryCooldown - 1);
      }
      if (state.ShieldRecoveryCooldown <= 0) {
        state.ShieldDamage = Math.Max(0, state.ShieldDamage - RegenRate);
      }
    }
    return state;
  }

  public void ApplyState(PlayerState state) {
    var shieldHealth = MaxShieldHealth - state.ShieldDamage;
    var shieldSizeRatio = shieldHealth / (float)MaxShieldHealth;
    _shieldTransform.localScale = Vector3.one * ShieldSize * shieldSizeRatio;
    if (TargetBone != null) {
      _shieldTransform.localPosition = transform.InverseTransformPoint(TargetBone.position);
    }
  }

  public PlayerState ResetState(PlayerState state) => state;

  //public override void ResetState(ref CharacterStateSummary state) {
      //state.ShieldHealth = MaxShieldHealth;
  //}

  //public override void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
      //context.ShieldHP = summary.ShieldHealth;
  //}

  void SetShieldColor(Color color) {
    foreach (var renderer in _shieldRenderers) {
      color.a = renderer.sharedMaterial.color.a;
      color = color * 0.75f;
      renderer.material.color = color;
      renderer.material.SetColor("_RimColor", color);
    }
  }

  void OnValidate() {
    ResetHealth = Mathf.Clamp(ResetHealth , 0f, MaxShieldHealth);
  }

}

}
