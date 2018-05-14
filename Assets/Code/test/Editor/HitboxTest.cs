using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class HitboxTest : HitboxTestBase {

  [TestCase(0, 0, 0)] [TestCase(0, 0, 10)] [TestCase(0, 0, 100)]
  [TestCase(0, 1, 0)] [TestCase(0, 1, 10)] [TestCase(0, 1, 100)]
  [TestCase(0, 5, 0)] [TestCase(0, 5, 10)] [TestCase(0, 5, 100)]
  [TestCase(0, -1, 0)] [TestCase(0, -1, 10)] [TestCase(0, -1, 100)]
  [TestCase(0, -5, 0)] [TestCase(0, -5, 10)] [TestCase(0, -5, 100)]
  public void GetKnockbackScale_cannot_be_negative(float baseKnockback, float scaling, float damage) {
    var scale = CreateHitbox().WithBaseKnockback(baseKnockback) .WithKnockbackScaling(scaling).Build().GetKnockbackScale(damage);
    Assert.That(scale, Is.GreaterThanOrEqualTo(0f));
  }

  [TestCase(0, 0, 0, 0)] [TestCase(0, 0, 10, 0)] [TestCase(0, 0, 100, 0)]
  [TestCase(0, 1, 0, 0)] [TestCase(0, 1, 10, 10)] [TestCase(0, 1, 100, 100)]
  [TestCase(0, 5, 0, 0)] [TestCase(0, 5, 10, 50)] [TestCase(0, 5, 100, 500)]
  public void GetKnockbackScale_scales_with_damage(float baseKnockback, float scaling, float damage, float knockback) {
    var scale = CreateHitbox().WithBaseKnockback(baseKnockback) .WithKnockbackScaling(scaling).Build().GetKnockbackScale(damage);
    Assert.AreEqual(scale, knockback, float.Epsilon);
  }

  [TestCase(5, 0, 0, 5)] [TestCase(5, 0, 10, 5)] [TestCase(5, 0, 100, 5)]
  [TestCase(5, 1, 0, 5)] [TestCase(5, 1, 10, 15)] [TestCase(5, 1, 100, 105)]
  [TestCase(5, 5, 0, 5)] [TestCase(5, 5, 10, 55)] [TestCase(5, 5, 100, 505)]
  public void GetKnockbackScale_has_unscaling_base_knockback(float baseKnockback, float scaling, float damage, float knockback) {
    var scale = CreateHitbox().WithBaseKnockback(baseKnockback) .WithKnockbackScaling(scaling).Build().GetKnockbackScale(damage);
    Assert.AreEqual(scale, knockback, float.Epsilon);
  }

  [TestCase(0, 0, 0)] [TestCase(0, 0, 10)] [TestCase(0, 0, 100)]
  [TestCase(0, 1, 0)] [TestCase(0, 1, 10)] [TestCase(0, 1, 100)]
  [TestCase(0, 5, 0)] [TestCase(0, 5, 10)] [TestCase(0, 5, 100)]
  [TestCase(0, -1, 0)] [TestCase(0, -1, 10)] [TestCase(0, -1, 100)]
  [TestCase(0, -5, 0)] [TestCase(0, -5, 10)] [TestCase(0, -5, 100)]
  public void GetHitstun_cannot_be_negative(int baseHitstun, float scaling, float damage) {
    var hitstun = CreateHitbox().WithBaseHitstun((uint)baseHitstun).WithHitstunScaling(scaling).Build().GetHitstun(damage);
    Assert.That(hitstun, Is.GreaterThanOrEqualTo(0f));
  }

  [TestCase(0, 0, 0, 0)] [TestCase(0, 0, 10, 0)] [TestCase(0, 0, 100, 0)]
  [TestCase(0, 1, 0, 0)] [TestCase(0, 1, 10, 10)] [TestCase(0, 1, 100, 100)]
  [TestCase(0, 5, 0, 0)] [TestCase(0, 5, 10, 50)] [TestCase(0, 5, 100, 500)]
  public void GetHitstun_scales_with_damage(int baseHitstun, float scaling, float damage, float knockback) {
    var hitstun = CreateHitbox().WithBaseHitstun((uint)baseHitstun).WithHitstunScaling(scaling).Build().GetHitstun(damage);
    Assert.AreEqual(hitstun, knockback, float.Epsilon);
  }

  [TestCase(5, 0, 0, 5)] [TestCase(5, 0, 10, 5)] [TestCase(5, 0, 100, 5)]
  [TestCase(5, 1, 0, 5)] [TestCase(5, 1, 10, 15)] [TestCase(5, 1, 100, 105)]
  [TestCase(5, 5, 0, 5)] [TestCase(5, 5, 10, 55)] [TestCase(5, 5, 100, 505)]
  public void GetHitstun_has_unscaling_base_hitsun(int baseHitstun, float scaling, float damage, float knockback) {
    var hitstun = CreateHitbox().WithBaseHitstun((uint)baseHitstun).WithHitstunScaling(scaling).Build().GetHitstun(damage);
    Assert.AreEqual(hitstun, knockback, float.Epsilon);
  }

  [TestCase(0, 1, 0)] [TestCase(90, 0, 1)] [TestCase(180, -1, 0)] [TestCase(270, 0, -1)] [TestCase(360, 1, 0)]
  [TestCase(45, 0.70710678118f, 0.70710678118f)] [TestCase(135, -0.70710678118f, 0.70710678118f)] 
  [TestCase(225, -0.70710678118f, -0.70710678118f)] [TestCase(315, 0.70710678118f, -0.70710678118f)] 
  public void GetKnockbackDirection_corresponds_to_knockback_angle(float angle, float x, float y) {
    var knockbackDirection = CreateHitbox().WithMirrorDirection(false).WithKnockbackAngle(angle).Build().GetKnockbackDirection(false);
    Assert.AreEqual(x, knockbackDirection.x, 0.000001);
    Assert.AreEqual(y, knockbackDirection.y, 0.000001);
    knockbackDirection = CreateHitbox().WithMirrorDirection(false).WithKnockbackAngle(angle).Build().GetKnockbackDirection(true);
    Assert.AreEqual(x, knockbackDirection.x, 0.000001);
    Assert.AreEqual(y, knockbackDirection.y, 0.000001);
  }

  [TestCase(0, 1, 0)] [TestCase(90, 0, 1)] [TestCase(180, -1, 0)] [TestCase(270, 0, -1)] [TestCase(360, 1, 0)]
  [TestCase(45, 0.70710678118f, 0.70710678118f)] [TestCase(135, -0.70710678118f, 0.70710678118f)] 
  [TestCase(225, -0.70710678118f, -0.70710678118f)] [TestCase(315, 0.70710678118f, -0.70710678118f)] 
  public void GetKnockbackDirection_mirrors_direction(float angle, float x, float y) {
    var knockbackDirection = CreateHitbox().WithMirrorDirection(true).WithKnockbackAngle(angle).Build().GetKnockbackDirection(false);
    Assert.AreEqual(-x, knockbackDirection.x, 0.000001);
    Assert.AreEqual(y, knockbackDirection.y, 0.000001);
    knockbackDirection = CreateHitbox().WithMirrorDirection(true).WithKnockbackAngle(angle).Build().GetKnockbackDirection(true);
    Assert.AreEqual(x, knockbackDirection.x, 0.000001);
    Assert.AreEqual(y, knockbackDirection.y, 0.000001);
  }

  [TestCase(0, 0, 0, 0)] [TestCase(0, 90, 0, 0)] [TestCase(0, 180, 0, 0)] [TestCase(0, 270, 0, 0)] [TestCase(0, 360, 0, 0)]
  [TestCase(0, 45, 0, 0)] [TestCase(0, 135, 0, 0)] [TestCase(0, 225, 0, 0)] [TestCase(0, 315, 0, 0)] 
  [TestCase(1, 0, 1, 0)] [TestCase(1, 90, 0, 1)] [TestCase(1, 180, -1, 0)] [TestCase(1, 270, 0, -1)] [TestCase(1, 360, 1, 0)]
  [TestCase(1, 45, 0.70710678118f, 0.70710678118f)] [TestCase(1, 135, -0.70710678118f, 0.70710678118f)] 
  [TestCase(1, 225, -0.70710678118f, -0.70710678118f)] [TestCase(1, 315, 0.70710678118f, -0.70710678118f)] 
  [TestCase(10, 0, 10, 0)] [TestCase(10, 90, 0, 10)] [TestCase(10, 180, -10, 0)] [TestCase(10, 270, 0, -10)] [TestCase(10, 360, 10, 0)]
  [TestCase(10, 45, 7.0710678118f, 7.0710678118f)] [TestCase(10, 135, -7.0710678118f, 7.0710678118f)] 
  [TestCase(10, 225, -7.0710678118f, -7.0710678118f)] [TestCase(10, 315, 7.0710678118f, -7.0710678118f)] 
  [TestCase(100, 0, 100, 0)] [TestCase(100, 90, 0, 100)] [TestCase(100, 180, -100, 0)] [TestCase(100, 270, 0, -100)] [TestCase(100, 360, 100, 0)]
  [TestCase(100, 45, 70.710678118f, 70.710678118f)] [TestCase(100, 135, -70.710678118f, 70.710678118f)] 
  [TestCase(100, 225, -70.710678118f, -70.710678118f)] [TestCase(100, 315, 70.710678118f, -70.710678118f)] 
  public void GetKnockback_scales_with_damage(float damage, float angle, float x, float y) {
  var knockback = CreateHitbox().WithBaseKnockback(0).WithKnockbackScaling(1)
                                .WithKnockbackAngle(angle).Build().GetKnocback(damage, true);
    Assert.AreEqual(x, knockback.x, 0.001);
    Assert.AreEqual(y, knockback.y, 0.001);
  }
 
	[Test]
	public void IsActive_returns_false_when_not_enabled() {
    var hitbox = CreateHitbox().WithEnabled(false).Build();
    Assert.IsFalse(hitbox.IsActive);
	}

	[Test]
	public void IsActive_returns_false_when_gameobject_inactive() {
    var hitbox = CreateHitbox().WithActive(false).Build();
    Assert.IsFalse(hitbox.IsActive);
	}

	[Test]
	public void IsActive_returns_false_when_type_is_inactive() {
    var hitbox = CreateHitbox().WithType(HitboxType.Inactive).Build();
    Assert.IsFalse(hitbox.IsActive);
	}

	[Test]
	public void IsActive_returns_true_when_enabled_and_has_good_type() {
    var hitbox = CreateHitbox().Build();
    Assert.IsTrue(hitbox.IsActive);
	}

	[Test]
	public void Center_is_transformed_by_transform_scale() {
    const float scale = 5f;
    var hitbox = CreateHitbox().WithOffset(Vector3.one).Build();
    hitbox.transform.localScale = Vector3.one * scale;
    Assert.AreEqual(Vector3.one * scale, hitbox.Center);
	}

	[Test]
	public void Center_is_transformed_by_transform_rotation() {
    var hitbox = CreateHitbox().WithOffset(Vector3.one).Build();
    hitbox.transform.Rotate(90, 0, 0);
    Assert.AreEqual(1.0, hitbox.Center.x, 0.0001);
    Assert.AreEqual(-1.0, hitbox.Center.y, 0.0001);
    Assert.AreEqual(1.0, hitbox.Center.z, 0.0001);
	}

	[Test]
	public void Center_is_transformed_by_transform_position() {
    var hitbox = CreateHitbox().WithOffset(Vector3.one).Build();
    hitbox.transform.Translate(5, 0, 0);
    Assert.AreEqual(new Vector3(6, 1, 1), hitbox.Center);
	}

	[Test]
	public void CollisionCheck_returns_zero_if_no_nearby_colliders() {
    var hitbox = CreateHitbox().WithOffset(Vector3.one).WithRadius(0.5f).Build();
    Assert.AreEqual(0, HitboxUtil.CollisionCheck(hitbox, new Hurtbox[10]));
	}

	[Test]
	public void CollisionCheck_returns_zero_if_colliders_has_no_hurtboxes() {
    var hitbox = CreateHitbox().WithOffset(Vector3.one).WithRadius(0.5f).Build();
    CreateObject<SphereCollider>().transform.position = -0.5f * Vector3.up;
    CreateObject<SphereCollider>().transform.position = 0.5f * Vector3.up;
    CreateObject<SphereCollider>().transform.position = 0.5f * Vector3.right;
    Assert.AreEqual(0, HitboxUtil.CollisionCheck(hitbox, new Hurtbox[10]));
	}

	[Test]
	public void CollisionCheck_ignores_disabled_hurtboxes() {
    var hitbox = CreateHitbox().WithOffset(Vector3.one).WithRadius(1.5f).Build();
    CreateHurtbox().WithPosition(-0.5f * Vector3.up).WithRadius(0.5f).WithEnabled(false);
    CreateHurtbox().WithPosition(0.5f * Vector3.up).WithRadius(0.5f);
    CreateObject<SphereCollider>().transform.position = 0.5f * Vector3.right;
    Assert.AreEqual(1, HitboxUtil.CollisionCheck(hitbox, new Hurtbox[10]));
	}

	[Test]
	public void CollisionCheck_ignores_hurtboxes_on_inactive_gameobjects() {
    var hitbox = CreateHitbox().WithRadius(1.5f).Build();
    CreateHurtbox().WithPosition(-0.5f * Vector3.up).WithRadius(0.5f);
    CreateHurtbox().WithPosition(0.5f * Vector3.up).WithRadius(0.5f).WithActive(false);
    CreateObject<SphereCollider>().transform.position = 0.5f * Vector3.right;
    Assert.AreEqual(1, HitboxUtil.CollisionCheck(hitbox, new Hurtbox[10]));
	}

	[Test]
	public void CollisionCheck_returns_number_of_collided_hurtboxes() {
    var hitbox = CreateHitbox().WithRadius(1.5f).Build();
    var h1 = CreateHurtbox().WithPosition(-0.5f * Vector3.up).WithRadius(0.5f).Build();
    var h2 = CreateHurtbox().WithPosition(0.5f * Vector3.up).WithRadius(0.5f).Build();
    CreateObject<SphereCollider>().transform.position = 0.5f * Vector3.right;
    var hurtboxes = new Hurtbox[10];
    Assert.AreEqual(2, HitboxUtil.CollisionCheck(hitbox, hurtboxes));
    CollectionAssert.AreEquivalent(new[] {h1, h2}, hurtboxes.Take(2));
	}

	[Test]
	public void CollisionCheck_returns_non_null_hurtboxes() {
    var hitbox = CreateHitbox().WithRadius(1.5f).Build();
    CreateHurtbox().WithPosition(-0.5f * Vector3.up).WithRadius(0.5f);
    CreateHurtbox().WithPosition(0.5f * Vector3.up).WithRadius(0.5f);
    CreateObject<SphereCollider>().transform.position = 0.5f * Vector3.right;
    var hurtboxes = new Hurtbox[10];
    var count = HitboxUtil.CollisionCheck(hitbox, hurtboxes);
    Assert.AreEqual(2, count);
    Assert.That(hurtboxes.Take(count), Is.Not.Null);
	}


}