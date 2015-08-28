using UnityEngine;
using System.Collections;

namespace Hourai {
	
	public class FollowObject : MonoBehaviour {
		
		public Transform Target;
		public Vector3 Offset;
		public bool UseLocalSpace;
		
		void LateUpdate() {
			if(!Target)
				return;
			transform.position = UseLocalSpace ? Target.TransformPoint(Offset) : Target.position + Offset;
		}
		
	}

}

