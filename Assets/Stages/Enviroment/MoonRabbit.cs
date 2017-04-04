using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonRabbit : MonoBehaviour {

    [Serializable]
    public struct LevelObjects
    {
        public string Name;
        public GameObject[] Objects;
        public GameObject[] LOD;// Each element must be parent objects
        public MonoBehaviour[] behaviour;

    }

    [SerializeField]
    Camera MainCamera;

    [SerializeField]
    LevelObjects[] levels;

    int qualityLevel;

    // Use this for initialization
    void Awake() {
        qualityLevel = QualitySettings.GetQualityLevel();
        foreach(var GameObject in levels[qualityLevel].Objects)
        {
            GameObject.SetActive(false);
        }
        foreach (var LODGroup in levels[qualityLevel].LOD)
        {
            foreach (var LODGO in LODGroup.GetComponentsInChildren<LODChanger>())
            {
                LODGO.GetComponent<LODChanger>().EnableLODs();
            }

        }
        foreach (var comp in levels[qualityLevel].behaviour)
        {
            comp.enabled = false;
        }

    }
	
}
