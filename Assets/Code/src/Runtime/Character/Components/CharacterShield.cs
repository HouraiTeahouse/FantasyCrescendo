using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[Serializable]
public sealed class CharacterShield : CharacterComponent {

  public GameObject ShieldPrefab;
  public Transform TargetBone;

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

  public bool IsShieldActive(in PlayerState state) => Character.GetControllerState(state) is ShieldState;
  public bool IsShieldBroken(in PlayerState state) => state.ShieldDamage >= MaxShieldHealth;

  public override void PreInit(Character character) {
    Shield = Object.Instantiate(ShieldPrefab);
    Shield.name = "Shield";
    ShieldTransform = Shield.transform;
    ShieldTransform.parent = character.transform;
    ShieldRenderers = Shield.GetComponentsInChildren<Renderer>();
    Shield.SetActive(false);
  }

  public override Task Init(Character character) {
    ResetHealth = Mathf.Clamp(ResetHealth , 0f, MaxShieldHealth);
    SetShieldColor(Config.Get<VisualConfig>().GetPlayerColor(character.Config.PlayerID));
    return Task.CompletedTask;
  }

  public override void Simulate(ref PlayerState state, in PlayerInputContext input) {
    if (IsShieldActive(state)) {
      state.ShieldDamage = Math.Min(MaxShieldHealth, state.ShieldDamage + DepletionRate);
      state.ShieldRecoveryCooldown = RecoveryCooldown;
    } else {
      state.ShieldRecoveryCooldown = (uint)Mathf.Max(0, (int)state.ShieldRecoveryCooldown - 1);
      if (state.ShieldRecoveryCooldown <= 0) {
        state.ShieldDamage = (uint)Math.Max(0, (int)state.ShieldDamage - RegenRate);
      }
    }
  }

  public override void UpdateView(in PlayerState state) {
    var shieldHealth = MaxShieldHealth - state.ShieldDamage;
    var shieldSizeRatio = shieldHealth / (float)MaxShieldHealth;
    ObjectUtil.SetActive(Shield, IsShieldActive(state));
    ShieldTransform.localScale = Vector3.one * ShieldScale * ShieldSize.Evaluate(shieldSizeRatio);
    if (TargetBone != null) {
      ShieldTransform.localPosition = Character.transform.InverseTransformPoint(TargetBone.position);
    }
  }

  public override void Dispose() {
    base.Dispose();
    if (Shield == null) {
      Object.Destroy(Shield);
    }
  }

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

}

}
