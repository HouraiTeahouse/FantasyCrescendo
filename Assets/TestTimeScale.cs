using UnityEngine;
using System.Collections;

public class TestTimeScale : MonoBehaviour {

    [Range(0f, 5f)]
    public float TimeScale = 1f;
	
	// Update is called once per frame
	void Update () {
        Time.timeScale = TimeScale;
	}
}
