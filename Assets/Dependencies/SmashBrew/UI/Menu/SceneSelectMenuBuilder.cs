using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// Constructs the maps select section of the in-match UI 
    /// </summary>
    public class SceneSelectMenuBuilder : AbstractSelectMenuBuilder<SceneData> {

        protected override IEnumerable<SceneData> GetData() {
            return DataManager.Instance.Scenes;
        }


    }
}
