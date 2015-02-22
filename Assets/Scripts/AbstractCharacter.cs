using UnityEngine;
using System.Collections;
using UnityUtilLib;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class AbstractCharacter : MonoBehaviour {
	
	private const string groundedTag = "Ground";

	[SerializeField]
	private float walkSpeed = 10;
	[SerializeField]
	private float runSpeed = 20;
	[SerializeField]
	private float jumpHeight = 20;
	[SerializeField]
	private int jumpCount = 2;
	[SerializeField]
	private float jumpDampening = 0.5f;

	private Rigidbody2D rigBod;
	[SerializeField]
	private int jumpsRemaining;
	private bool grounded;
	public bool IsGrounded {
		get {
			return grounded;
		}
	}

	//TODO: implement
	private bool running;
	private bool hit;

	private void Awake() {
		rigBod = rigidbody2D;
		hit = false;
	}

	//FixedUpdate is for handling physics/control
	protected virtual void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		float m = rigBod.mass;
		Vector2 v = rigBod.velocity;

		float horizontalSpeed = (running) ? runSpeed : walkSpeed;
		Vector2 movementForce = Vector2.right * (horizontalSpeed - v.magnitude);
		movementForce.x *= Util.Sign (horizontal);
		rigBod.AddForce (movementForce);
		//If on the ground
		if(grounded || jumpsRemaining > 0) {
			if(Input.GetButtonDown("Vertical") && vertical > 0f && jumpsRemaining > 0){
				Jump (jumpHeight * Mathf.Pow(jumpDampening, jumpCount - jumpsRemaining), dt);
				--jumpsRemaining;
			}
		}

		Debug.Log ("" + horizontal + " " + v + " " + movementForce);
	}

	void OnCollisionEnter2D(Collision2D col) {
		jumpsRemaining = jumpCount;
		GroundedCheck (col.gameObject, true);
	}

	void OnCollisionExit2D(Collision2D col) {
		GroundedCheck (col.gameObject, false);
	}

	private void GroundedCheck(GameObject collidedObject, bool groundedValue) {
		if (collidedObject.CompareTag(groundedTag)) {
			Debug.Log((groundedValue) ? "Grounded" : "No Longer Grounded");
			grounded = groundedValue;
		}
	}

	private void Jump(float height, float dt) {
		Vector2 v = rigBod.velocity;
		float m = rigBod.mass;
		float g = rigBod.gravityScale * Physics2D.gravity.magnitude;
		// 1/2mv^2 = mgh
		// v^2 = 2gh
		// v = sqrt(2gh)
		float v0 = Mathf.Sqrt(2f * g * height);
		// f = ma
		// a = (vf - v0) / dt
		rigBod.AddForce(Vector2.up * m * (v0 - v.y) / dt);
	}
}
