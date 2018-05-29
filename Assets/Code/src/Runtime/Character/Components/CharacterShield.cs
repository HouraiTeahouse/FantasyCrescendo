using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[RequireComponent(typeof(CharacterStateMachine))]
public sealed class CharacterShield : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public CharacterStateMachine StateMachine;
  public GameObject ShieldPrefab;
  public Transform TargetBone;

  // Character Constrants
  [Header("Constants")]
  [Tooltip("How large the shield is while the player's shield is full.")]
  public float ShieldScale = 1f;

  [Tooltip("The size of the shield (0 to 1) for a given health ratio (0 to 1)")]
  public AnimationCurve ShieldSize = AnimationCurve.Linear(0, 0, 1, 1);

  [Tooltip("Maximum shield health for the character.")]
  public uint MaxShieldHealth = 10000;

  [Tooltip("How much shield health is lost over time.")]
  public uint DepletionRate = 250;

  [Tooltip("How fast shield health replenishes when not in use.")]
  public uint RegenRate = 200;

  public uint RecoveryCooldown = 120;

  [Tooltip("How much health the shield resets to after being broken.")]
  public float ResetHealth = 30f;

  GameObject Shield;
  Transform ShieldTransform;
  Hurtbox ShieldHurtbox;
  Renderer[] ShieldRenderers;
  bool isShieldView;

  public bool IsShieldActive(ref PlayerState state) => StateMachine.GetControllerState(ref state) is ShieldState;
  public bool IsShieldBroken(ref PlayerState state) => state.ShieldDamage >= MaxShieldHealth;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    // NOTE: This is only to allow the shield components to be stripped before
    // charater initialization. Do not do this in other situations.

    // Create Shield Object
    Shield = Instantiate(ShieldPrefab);
    Shield.name = "Shield";
    ShieldTransform = Shield.transform;
    ShieldTransform.parent = transform;
    ShieldRenderers = Shield.GetComponentsInChildren<Renderer>();
    Shield.SetActive(false);
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    if (Shield == null) {
      Destroy(Shield);
    }
  }

  public Task Initialize(PlayerConfig config, bool isView) {
    SetShieldColor(Config.Get<VisualConfig>().GetPlayerColor(config.PlayerID));
    return Task.CompletedTask;
  }

  public void Presimulate(ref PlayerState state) => ApplyState(ref state);

  public void Simulate(ref PlayerState state, PlayerInputContext input) {
    if (IsShieldActive(ref state)) {
      state.ShieldDamage = Math.Min(MaxShieldHealth, state.ShieldDamage + DepletionRate);
      state.ShieldRecoveryCooldown = RecoveryCooldown;
    } else {
      state.ShieldRecoveryCooldown = (uint)Mathf.Max(0, (int)state.ShieldRecoveryCooldown - 1);
      if (state.ShieldRecoveryCooldown <= 0) {
        state.ShieldDamage = (uint)Math.Max(0, (int)state.ShieldDamage - RegenRate);
      }
    }
  }

  public void ApplyState(ref PlayerState state) {
    var shieldHealth = MaxShieldHealth - state.ShieldDamage;
    var shieldSizeRatio = shieldHealth / (float)MaxShieldHealth;
    ObjectUtil.SetActive(Shield, IsShieldActive(ref state));
    ShieldTransform.localScale = Vector3.one * ShieldScale * ShieldSize.Evaluate(shieldSizeRatio);
    if (TargetBone != null) {
      ShieldTransform.localPosition = transform.InverseTransformPoint(TargetBone.position);
    }
  }

  public void ResetState(ref PlayerState state) {}

  void SetShieldColor(Color color) {
    foreach (var renderer in ShieldRenderers) {
      if (renderer == null) continue;
      var shieldColor = color;
      shieldColor.a = renderer.sharedMaterial.color.a;
      shieldColor = shieldColor * 0.75f;
      renderer.material.color = shieldColor;
      renderer.material.SetColor("_RimColor", shieldColor);
    }
  }

  void OnValidate() {
    ResetHealth = Mathf.Clamp(ResetHealth , 0f, MaxShieldHealth);
  }

}

}
