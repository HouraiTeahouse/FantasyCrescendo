using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    
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

            foreach (SceneData stage in dataManager.Scenes.Where(scene => scene.IsStage)) {
                GameObject go = Instantiate(stageSlotButton);
                var stu = go.GetComponent<StageSlotUI>();
                if (stu == null) {
                    Debug.LogError("The Stage Loader can't find the data manager object in the scene.");
                    return;
                }
                stu.setStageName(stage.Name);
                stu.setStageLogoElement(stageLogo);
                stu.setStageTextElement(stageNameText);
                go.transform.SetParent(sectionToFill.transform, false);
            }
        }

        // Update is called once per frame
        private void Update() { }

    }

}
