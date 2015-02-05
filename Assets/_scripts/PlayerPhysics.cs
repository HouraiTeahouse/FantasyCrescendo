using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour {

	public LayerMask collisionMask;

	private BoxCollider colider;
	private Vector3 size;
	private Vector3 centre;

	private float skin = .005f;

	public bool standing;
	private bool dropdown;

	Ray ray;
	RaycastHit hit;

	void Start()
	{
		colider = GetComponent<BoxCollider>();
		size = colider.size;
		centre = colider.center;
		dropdown = false;
	}

	public void move(Vector2 movement)
	{
		float deltaY = movement.y;
		float deltaX = movement.x;
		Vector2 p = transform.position;

		for (int i =0; i<3; ++i) {
			float dir = Mathf.Sign(deltaY);
			float x = (p.x + centre.x - size.x/2) + size.x/2 * i;
			float y = p.y + centre.y + size.y/2 * dir;

			ray = new Ray(new Vector2(x,y), new Vector2(0,dir));
			Debug.DrawRay(ray.origin,ray.direction);
					
			if(dir<0 & Physics.Raycast(ray,out hit,Mathf.Abs(deltaY),collisionMask))
			{
				standing = true;
				float dst = Vector3.Distance (ray.origin, hit.point);
					
				if(dst>skin)
				{
					deltaY = dst*dir + skin;
				} else {
					deltaY = 0;
				}
				break;
			} else {
				standing = false;
			}
		}

		if (dropdown) {
			dropdown = false;
			deltaY -= 0.05f;
		}

		transform.Translate (new Vector3(deltaX,deltaY,0));
	}

	//Drop the guy down the ramp
	public void dropDown()
	{
		dropdown = true;
	}
}
