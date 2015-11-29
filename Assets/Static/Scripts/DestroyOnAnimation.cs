using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class DestroyOnAnimation : MonoBehaviour {

    private Animation _animation;

    void Awake() {
        _animation = GetComponent<Animation>();
    }

	// Update is called once per frame
	void Update () {
	    if(!_animation || !_animation.isPlaying)
            Destroy(gameObject);
	}
}
