using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Crescendo.API;

public class CursorController : MonoBehaviour {

	public float movSpeed = 100.0f;
	public float vx = 0.0f;
	public float vy = 0.0f;
	public float smooth = 100.0f;

	public StandaloneInputModule mouseControls;
	public CursorInputModule keyboardControls;

	private ICharacterInput InputSource;
	
	void OnEnable()
	{
		mouseControls.enabled = false;
		keyboardControls.enabled = true;
	}

	void OnDisable()
	{
		mouseControls.enabled = true;
		keyboardControls.enabled = false;
	}

	// Use this for initialization
	void Start () {
		InputSource = this.GetComponent<TestInput>();
	}
	
	// Update is called once per frame
	void Update () {
		//float inputX = Input.GetAxisRaw ("Horizontal");
		float inputX = InputSource.Movement.x;
		float targetSpeed = Mathf.Min (Mathf.Abs(inputX), 1.0f);
		targetSpeed = movSpeed*targetSpeed;
		if( inputX < 0.0f )
		{
			targetSpeed = -1.0f*targetSpeed;
		}
		vx = Mathf.Lerp( vx, targetSpeed, smooth*Time.deltaTime );


		//float inputY = Input.GetAxisRaw ("Vertical");
		float inputY = InputSource.Movement.y;
		targetSpeed = Mathf.Min (Mathf.Abs(inputY), 1.0f);
		targetSpeed = movSpeed*targetSpeed;
		if( inputY < 0.0f )
		{
			targetSpeed = -1.0f*targetSpeed;
		}
		vy = Mathf.Lerp( vy, targetSpeed, smooth*Time.deltaTime );


		transform.Translate( vx*Time.deltaTime, vy*Time.deltaTime, 0.0f  );
	}
}
