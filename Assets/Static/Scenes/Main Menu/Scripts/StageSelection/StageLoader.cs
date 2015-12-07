using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {
    
    public class StageLoader : MonoBehaviour {

        public GameObject sectionToFill = null;
        public Image stageLogo = null;
        public Text stageNameText = null;
        public GameObject stageSlotButton = null;

        // Use this for initialization
        private void Start() {
            DataManager dataManager = DataManager.Instance;

            if (stageSlotButton == null || sectionToFill == null || stageLogo == null || stageNameText == null) {
                Debug.LogError("Fill all game objects needed by the Stage Loader.");
                return;
            }

            if (dataManager == null) {
                Debug.LogError("The Stage Loader can't find the data manager object in the scene.");
                return;
            }

            List<string> stageNames = dataManager.GetAvailableStages();
            var i = 0;
            for (i = 0; i < stageNames.Count; i++) {
                GameObject go = Instantiate(stageSlotButton);
                var stu = go.GetComponent<StageSlotUI>();
                if (stu == null) {
                    Debug.LogError("The Stage Loader can't find the data manager object in the scene.");
                    return;
                }
                stu.setStageName(stageNames[i]);
                stu.setStageLogoElement(stageLogo);
                stu.setStageTextElement(stageNameText);
                go.transform.SetParent(sectionToFill.transform, false);
            }
        }

        // Update is called once per frame
        private void Update() { }

    }

}
