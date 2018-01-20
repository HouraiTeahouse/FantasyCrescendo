using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class HitboxTest : HitboxTestBase {

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
    Assert.AreEqual(h1, hurtboxes[1]);
    Assert.AreEqual(h2, hurtboxes[0]);
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