using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

public struct HitboxInfo {

    public bool IsActive => Type != HitboxType.Inactive;

    public HitboxType Type;
    public Vector3 Offset;
    public float Radius;

    public uint Priority;

    public LinearScaledValue Damage;
    public LinearScaledValue Knockback;
    public LinearScaledValue Hitstun;
    [Range(-180f, 180f)] public float KnockbackAngle;
    public bool MirrorDirection;


    public SoundEffect[] SFX;
    public GameObject[] VisualEffects;

    public uint Hitlag {
        get {
            var damage = Mathf.Abs(Damage.GetScaledValue(0f));
            var hitlag = Config.Get<PhysicsConfig>().Hitlag.GetScaledValue(damage);
            return (uint)Mathf.Max(0, Mathf.FloorToInt(hitlag));
        }
    }

    float KnockbackAngleRad => Mathf.Deg2Rad * KnockbackAngle;

    public Vector2 GetKnockbackDirection(bool direction) {
        var dirRad = Mathf.Deg2Rad * KnockbackAngle;
        var dirMult = 1f;
        if (MirrorDirection && !direction) {
            dirMult = -1;
        }
        return new Vector2(dirMult * Mathf.Cos(KnockbackAngleRad), Mathf.Sin(KnockbackAngleRad));
    }

    public float GetKnockbackScale(float damage) {
        return Config.Get<PhysicsConfig>().GlobalKnockbackScaling * Mathf.Max(0, Knockback.GetScaledValue(damage));
    }

    public Vector2 GetKnocback(float damage, bool dir) => GetKnockbackScale(damage) * GetKnockbackDirection(dir);

    public uint GetHitstun(float damage) => (uint)Mathf.Max(0, Mathf.FloorToInt(Hitstun.GetScaledValue(damage)));

    public void PlayEffect(HitInfo hitInfo) {
        var position = hitInfo.Position;
        foreach (var effect in SFX) {
            if (effect != null) {
                effect.Play(position);
            }
        }

        foreach (var effect in VisualEffects) {
            var instance = PrefabPool.Get(effect)?.Rent();
            if (instance == null) continue;
            instance.transform.position = position;
            foreach (var vfx in instance.GetComponentsInChildren<HitEffect>()) {
                vfx.Setup(hitInfo);
            }
        }
    }

}

}
