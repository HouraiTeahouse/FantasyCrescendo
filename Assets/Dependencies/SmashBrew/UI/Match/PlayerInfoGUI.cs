using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PrefabFactoryEventHandler that creates </summary>
    public sealed class PlayerInfoGUI : MonoBehaviour {

        [SerializeField]
        RectTransform _prefab;

        /// <summary> The parent RectTransform to attach the spawned objects to. </summary>
        [SerializeField]
        RectTransform _container;

        RectTransform _finalSpace;

        /// <summary> The space prefabs to place before and after all of the elements to keep them centered. </summary>
        [SerializeField]
        RectTransform _spacePrefab;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            if (!_container) {
                enabled = false;
                return;
            }
            if (!_spacePrefab)
                return;

            RectTransform initialSpace = Instantiate(_spacePrefab);
            _finalSpace = Instantiate(_spacePrefab);

            initialSpace.SetParent(_container.transform);
            _finalSpace.SetParent(_container.transform);

            initialSpace.name = _spacePrefab.name;
            _finalSpace.name = _spacePrefab.name;

            foreach (var player in PlayerManager.Instance.MatchPlayers) {
                RectTransform display = Instantiate(_prefab);
                display.transform.SetParent(_container.transform, false);
                LayoutRebuilder.MarkLayoutForRebuild(display);
                display.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                display.name = "Player {0} Display".With(player.ID + 1);
                display.gameObject.SetActive(player.Type.IsActive);
                player.Changed += () => display.gameObject.SetActive(player.Type.IsActive);
                _finalSpace.transform.SetAsLastSibling();
            }
        }

    }

}
