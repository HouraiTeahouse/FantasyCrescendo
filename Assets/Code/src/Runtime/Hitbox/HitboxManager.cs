using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;

namespace HouraiTeahouse.FantasyCrescendo {

public class HitboxManager : MonoBehaviour {

    int _latestHitboxId;
    Dictionary<int, Transform> _hitboxBindSites;
    NativeList<Hitbox> _hitboxes;

    void Awake() {
        _latestHitboxId = 0;
        _hitboxBindSites = new Dictionary<int, Transform>();
        _hitboxes = new NativeList<Hitbox>(Allocator.Persistent);
    }

    void OnDestroy() {
        // _hitboxes.Dipose();
    }

    /// <summary>
    /// Destroys all hitboxes bound to a player.
    /// Projectiles and other non-bound hitboxes will not be destroyed.
    /// </summary>
    /// <param name="playerId"></param>
    public void FlushPlayerHitboxes(int playerId) {
        var idx = 0;
        while (idx < _hitboxes.Length) {
            var hitbox = _hitboxes[idx];
            if (hitbox.PlayerId == playerId && hitbox.BindId != 0) {
                _hitboxes.RemoveAtSwapBack(idx);
            } else {
                idx++;
            }
        }
    }

    /// <summary>
    /// Ages all hitboxes by a given number of frames and
    /// destroys the ones that no longer are alive.
    /// </summary>
    public void Step(int frames) {
        var idx = 0;
        while (idx < _hitboxes.Length) {
            var hitbox = _hitboxes[idx];
            hitbox.Lifetime -= frames;
            if (hitbox.Lifetime <= 0) {
                _hitboxes.RemoveAtSwapBack(idx);
            } else {
                _hitboxes[idx] = hitbox;
                idx++;
            }
        }
    }

    public void RegisterBindSite(int playerId, HitboxBindSite site) {
        Assert.IsNotNull(site);
        var runtimeBindId = GetRuntimeBindId(playerId, site.Id);
        Assert.IsFalse(_hitboxBindSites.ContainsKey(runtimeBindId));
        _hitboxBindSites.Add(runtimeBindId, site.transform);
    }

    public static int GetRuntimeBindId(int playerId, int bindId) =>
        playerId ^ bindId;

    public Hitbox CreateHitox(int playerId, int bindId, in HitboxInfo info) {
        var id = ++_latestHitboxId;
        var runtimeBindId = GetRuntimeBindId(playerId, bindId);
        Assert.IsTrue(_hitboxBindSites.ContainsKey(runtimeBindId));
        return new Hitbox {
            Id = id,
            PlayerId = playerId,
            BindId = runtimeBindId
        };
    }

}

public struct Hitbox {
    public int Id;
    public int PlayerId;
    public int BindId;
    public int Lifetime;

    public HitboxInfo Info;

    public Vector3 Position;
    public Vector3? OldPosition;

    public void UpdatePosition(Vector3 position) {
        OldPosition = Position;
        Position = position;
    }

    public uint Hitlag {
        get {
            var damage = Mathf.Abs(Info.Damage.GetScaledValue(0f));
            var hitlag = Config.Get<PhysicsConfig>().Hitlag.GetScaledValue(damage);
            return (uint)Mathf.Max(0, Mathf.FloorToInt(hitlag));
        }
    }

    public Vector2 GetKnockbackDirection(bool direction) {
        var dirRad = Mathf.Deg2Rad * Info.KnockbackAngle;
        var dirMult = 1f;
        if (Info.MirrorDirection && !direction) {
            dirMult = -1;
        }
        return new Vector2(dirMult * Mathf.Cos(dirRad), Mathf.Sin(dirRad));
    }

    public float GetKnockbackScale(float damage) {
        return Config.Get<PhysicsConfig>().GlobalKnockbackScaling * 
               Mathf.Max(0, Info.Knockback.GetScaledValue(damage));
    }

    public Vector2 GetKnocback(float damage, bool dir) => 
        GetKnockbackScale(damage) * GetKnockbackDirection(dir);

    public uint GetHitstun(float damage) => (uint)Mathf.Max(0, Mathf.FloorToInt(Info.Hitstun.GetScaledValue(damage)));

    public int GetCollidedHurtboxes(Hurtbox[] hurtboxes) {
        HitboxUtil.FlushHurtboxDedupCache();
        var arrayPool = ArrayPool<Collider>.Shared;
        var colliders = arrayPool.Rent(hurtboxes.Length);
        var colliderCount = FullCollisionCheck(colliders);
        int hurtboxCount = 0;
        for (int i = 0; i < colliderCount && hurtboxCount < hurtboxes.Length; i++) {
            if (colliders[i] == null) continue;
            var hurtbox = colliders[i].GetComponent<Hurtbox>();
            if (HitboxUtil.IsValidHurtbox(hurtbox)) {
                hurtboxes[hurtboxCount++] = hurtbox;
            }
        }
        arrayPool.Return(colliders);
        return hurtboxCount;
    }

    int StaticCollisionCheck(Collider[] colliders) {
        var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;
        return Physics.OverlapSphereNonAlloc(Position, Info.Radius, colliders, layerMask, 
                                             QueryTriggerInteraction.Collide);
    }

    int InterpolatedCollisionCheck(Collider[] colliders) {
        if (!OldPosition.HasValue) return 0;
        var arrayPool = ArrayPool<RaycastHit>.Shared;

        var hits = arrayPool.Rent(colliders.Length);
        var diff = Position - OldPosition.Value;
        var distance = diff.magnitude;
        var direction = diff.normalized;
        var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;

        var count = Physics.SphereCastNonAlloc(Position, Info.Radius, direction, hits, distance, 
                                               layerMask, QueryTriggerInteraction.Collide);
        for (var i = 0; i < count; i++) {
            colliders[i] = hits[i].collider;
        }

        arrayPool.Return(hits);
        return count;
    }

    int FullCollisionCheck(Collider[] colliders) {
        var arrayPool = ArrayPool<Collider>.Shared;
        var staticResults = arrayPool.Rent(colliders.Length);
        var interpolatedResults = arrayPool.Rent(colliders.Length);

        var staticCount = StaticCollisionCheck(staticResults);
        var interpolatedCount = InterpolatedCollisionCheck(interpolatedResults);

        Array.Copy(staticResults, colliders, staticCount);
        Array.Copy(interpolatedResults, 0, colliders, staticCount, Mathf.Min(interpolatedCount, colliders.Length - staticCount));

        arrayPool.Return(staticResults);
        arrayPool.Return(interpolatedResults);

        return ArrayUtility.RemoveDuplicates(colliders);
    }

}

}
