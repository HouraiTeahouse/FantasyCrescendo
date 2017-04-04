using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODChanger : MonoBehaviour {

    [SerializeField]
    MeshRenderer []HighQualityLOD;

    [SerializeField]
    MeshRenderer[]LowQualityLOD;

    // Use this for initialization
    public void EnableLODs () {

            foreach (var render in HighQualityLOD)
            {
                render.enabled = false;
                
            }
            foreach (var render in LowQualityLOD)
            {
                
                render.enabled = true;
            }
        }
	

}
