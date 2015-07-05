using UnityEngine;
using System;

/// <summary>
/// General character class for handling the physics and animations of individual characters
/// </summary>
/// Author: James Liu
/// Authored on 07/01/2015
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class Character : MonoBehaviour {

    [SerializeField]
    private int maxJumps;

    [SerializeField]
    private AnimationCurve jumpPower;

    protected Rigidbody Rigidbody { get; private set; }
    protected Animator Animator { get; private set; }
    protected CapsuleCollider Collider { get; private set; }

    public int PlayerNumber { get; set; }
    public Transform RespawnPosition { get; set; }

    public float Height {
        get {
            return Collider != null ? Collider.height : 0;
        }
        protected set {
            if (Collider != null)
                Collider.height = value;
        }
    }

    public int JumpCount { get; private set; }

    protected virtual void Awake() {
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        Collider = GetComponent<CapsuleCollider>();
    }

    public void Jump() {
        if (JumpCount < maxJumps) {
            if (maxJumps <= 0) {
                Rigidbody.AddForce(transform.up*jumpPower.Evaluate(0f));
            } else {
                Rigidbody.AddForce(transform.up * jumpPower.Evaluate((float)JumpCount / ((float)maxJumps - 1)));   
            }
            JumpCount++;
        }
    }

}
