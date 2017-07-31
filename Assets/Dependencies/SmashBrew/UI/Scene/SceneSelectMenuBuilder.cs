using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> Constructs the maps select section of the in-match UI </summary>
    public class SceneSelectMenuBuilder : AbstractSelectMenuBuilder<SceneData> {

        [SerializeField]
        bool _requireSelectable;

        [SerializeField]
        bool _includeMenus;

        [SerializeField]
        bool _includeStages = true;

        [SerializeField]
        bool _includeDebugScenes;

        protected override IEnumerable<SceneData> GetData() { 
            return DataManager.Scenes.Where(s => s.IsVisible);
        }

        protected override bool CanShowData(SceneData data) {
            if (_requireSelectable && !data.IsSelectable)
                return false;
            if (data.Type == SceneType.Menu && !_includeMenus)
                return false;
            if (data.Type == SceneType.Stage && !_includeStages)
                return false;
            return true;
        }

    }

}
