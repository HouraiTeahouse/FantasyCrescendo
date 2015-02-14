using UnityEngine;
using System.Collections;

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
	private float jumpCount = 2;

	private Rigidbody2D rigBod;
	private int jumpsRemaining;
	private bool grounded;

	//TODO: implement
	private bool running;
	private bool hit;

	void Start () {
		rigBod = rigidbody2D;
		hit = false;
	}

	//Update is for visual update only
	void Update ()
	{
//		targetSpeed = horizontal * walkspeed;
//		currentSpeed = IncrementTowards (currentSpeed, targetSpeed, acceleration);
//
//		amountToOffset.x = currentSpeed;
//		if (playerPhysics.standing) {
//			amountToOffset.y = 0;
//			if (vertical>0) {
//				amountToOffset.y = jumpHeight;
//			}
//			if (vertical<0){
//				playerPhysics.dropDown();
//			}
//		}
//		amountToOffset.y -= gravity * Time.deltaTime;
//
//		playerPhysics.move (amountToOffset * Time.deltaTime);
	}

	//FixedUpdate is for handling physics/control
	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		float m = rigBod.mass;
		float g = rigBod.gravityScale * Physics2D.gravity.magnitude;
		Vector2 v = rigBod.velocity;

		float horizontalSpeed = (running) ? runSpeed : walkSpeed;
		Vector2 movementForce = Vector2.right * m * (horizontalSpeed - v.x) / dt;
		if(horizontal == 0) {
			movementForce *= 0;
		} else if(horizontal < 0) {
			movementForce *= -1;
		}
		if(Input.GetButtonDown("Vertical") && vertical > 0f && grounded){
			// 1/2mv^2 = mgh
			// v^2 = 2gh
			// v = sqrt(2gh)
			float v0 = Mathf.Sqrt(2f * g * jumpHeight);
			// f = ma
			// a = (vf - v0) / dt
			movementForce += Vector2.up * m * (v0 - v.y) / dt;
		}

		rigBod.AddForce (movementForce);
		//Debug.Log ("" + horizontal + " " + rigBod.velocity + " " + movementForce);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if(col.gameObject.CompareTag(groundedTag)) {
			Debug.Log("Grounded");
			grounded = true;
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.CompareTag(groundedTag)) {
			Debug.Log("No Longer Grounded");
			grounded = false;
		}
	}

//	//increse the provided value towards the desired one
//	private float IncrementTowards(float arg, float target, float rate)
//	{
//		if (arg == target) {
//			return arg;
//		} else {
//			float dir = Mathf.Sign(target - arg);
//			arg += rate * Time.deltaTime * dir;
//			return(dir == Mathf.Sign(target - arg))? arg: target;
//		}
//	}

}
