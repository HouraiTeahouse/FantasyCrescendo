using UnityEngine;
using System.Linq;

namespace HouraiTeahouse.SmashBrew {

    public class BootstrapSceneLoader : MonoBehaviour {

        void Start() {
            var log = Log.GetLogger(this);
            DataManager.LoadTask.Then(() => {
                var scenes = DataManager.Scenes.OrderByDescending(s => s.Type).ThenByDescending(s => s.LoadPriority);
                var logStr = "Scene Considerations: ";
                foreach (var scene in scenes)
                    logStr += "\n   {0}: {1} {2}, Loadable: {3}".With(scene.name, scene.Type, scene.LoadPriority, scene.IsSelectable);
                log.Info(logStr);
                var startScene = scenes.FirstOrDefault(s => s.IsSelectable);
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

