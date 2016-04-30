using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// Constructs the maps select section of the in-match UI 
    /// </summary>

    public class SceneSelectBuilder : Builder {
        [Header("Select")] [SerializeField] private RectTransform _mapContainer;

        [SerializeField] private RectTransform _map;
        /// <summary>
        /// Construct the select area for maps.
        /// </summary>
        public override void CreateSelect ()
        {
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null || !_mapContainer || !_map)
                return;

            foreach (var data in dataManager.Scenes) {
                if (data == null || !data.IsStage)
                    continue;
                RectTransform map = Instantiate(_map);
                Attach(map, _mapContainer);
                map.name = data.name;
                map.GetComponentsInChildren<IDataComponent<SceneData>>().SetData(data);
            }
        }
    }
}