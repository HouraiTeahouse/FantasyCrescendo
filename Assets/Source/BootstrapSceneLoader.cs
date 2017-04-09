using UnityEngine;
using System.Linq;

namespace HouraiTeahouse.SmashBrew {

    public class BootstrapSceneLoader : MonoBehaviour {

        void Awake() {
            var log = Log.GetLogger(this);
            DataManager.LoadTask.Then(() => {
                var scenes = DataManager.Scenes.OrderByDescending(s => s.Type).ThenByDescending(s => s.LoadPriority);
                var logStr = "Scene Considerations: ";
                foreach (var scene in scenes)
                    logStr += "\n   {0}: {1} {2}".With(scene.name, scene.Type, scene.LoadPriority);
                log.Info(logStr);
                var startScene = scenes.FirstOrDefault();
                if (startScene == null)
                    log.Error("No usable loadable scene found.");
                else {
                    log.Info("Loading {0} as the initial scene...".With(startScene.name));
                    startScene.Load();
                }
            });
        }

    }

}

