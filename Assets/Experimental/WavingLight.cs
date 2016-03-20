using UnityEngine;
using System.Collections;

public class WavingLight : MonoBehaviour {

    public Light _light;
    public float _amplitude = 0.5f;
    public float _freqency = 1f;
	
	// Update is called once per frame
	void Update () {
	    if (_light)
	        _light.intensity = _amplitude * (1 + Mathf.Sin(_freqency * Time.time));
	}
}
