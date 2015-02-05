using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerInputScript : MonoBehaviour {

	public float walkspeed = 10;
	public float acceleration = 8;
	public float gravity = 20;
	public float jumpHeight = 20;

	private float currentSpeed;
	private float targetSpeed;
	private Vector2 amountToOffset;

	private PlayerPhysics playerPhysics;

	void Start ()
	{
		playerPhysics = GetComponent<PlayerPhysics> ();
	}

	void Update ()
	{
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

		targetSpeed = horizontal * walkspeed;
		currentSpeed = IncrementTowards (currentSpeed, targetSpeed, acceleration);

		amountToOffset.x = currentSpeed;
		if (playerPhysics.standing) {
			amountToOffset.y = 0;
			if (vertical>0) {
				amountToOffset.y = jumpHeight;
			}
			if (vertical<0){
				playerPhysics.dropDown();
			}
		}
		amountToOffset.y -= gravity * Time.deltaTime;

		playerPhysics.move (amountToOffset * Time.deltaTime);
	}

	//increse the provided value towards the desired one
	private float IncrementTowards(float arg, float target, float rate)
	{
		if (arg == target) {
			return arg;
		} else {
			float dir = Mathf.Sign(target - arg);
			arg += rate * Time.deltaTime * dir;
			return(dir == Mathf.Sign(target - arg))? arg: target;
		}
	}

}
