using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    public class LODManager : MonoBehaviour {

        [Serializable]
        public struct LevelObjects {
            public string Name;
            public GameObject[] Objects;
            public GameObject[] LOD;// Each element must be parent objects
            public MonoBehaviour[] Behaviours;
        }

        [SerializeField]
        LevelObjects[] levels;

        // Use this for initialization
        void Awake() {
            int qualityLevel = QualitySettings.GetQualityLevel();
            qualityLevel = Mathf.Clamp(qualityLevel, 0, levels.Length - 1);
            foreach (var gameObj in levels[qualityLevel].Objects)
                gameObj.SetActive(false);
            foreach (var group in levels[qualityLevel].LOD)
                foreach (var gameObj in group.GetComponentsInChildren<LODChanger>())
                    gameObj.EnableLODs();
            foreach (var comp in levels[qualityLevel].Behaviours)
                comp.enabled = false;
        }

    }

}
