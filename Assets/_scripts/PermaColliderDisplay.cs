using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class PermaColliderDisplay : MonoBehaviour {

	public Color boxColor;

	void Start() {
# if UNITY_EDITOR
		//Don't do any thing if it's in the editor
# else
		Destroy(this);
# endif
	}

	void OnDrawGizmos() {
		Color temp = Gizmos.color;
		Gizmos.color = boxColor;
		BoxCollider2D[] boxColliders = GetComponentsInChildren<BoxCollider2D>();
		for(int i = 0; i < boxColliders.Length; i++) {
			Transform objTrans = boxColliders[i].transform;
			Bounds colliderBounds = boxColliders[i].bounds;
			Gizmos.DrawWireCube(colliderBounds.center, colliderBounds.size);
		}
		Gizmos.color = temp;
	}

}
