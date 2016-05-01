using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    public abstract class AbstractSelectMenuBuilder<T> : MonoBehaviour where T : ScriptableObject, IGameData {
        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _prefab;

        protected virtual void Awake() {
            CreateSelect ();
        }

        public static void Attach(RectTransform child, Transform parent) {
            child.SetParent(parent, false);
            LayoutRebuilder.MarkLayoutForRebuild(child);
        }

        /// <summary>
        /// Construct the select area for characters.
        /// </summary>
        void CreateSelect () {
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null || !_container || !_prefab)
                return;

            foreach (T data in GetData()) {
                if (data == null || !CanShowData(data))
                    continue;
                RectTransform character = Instantiate(_prefab);
                Attach(character, _container);
                character.name = data.name;
                character.GetComponentsInChildren<IDataComponent<T>>().SetData(data);
                LogCreation(data);
            }
        }

        protected abstract IEnumerable<T> GetData();

        protected virtual bool CanShowData(T data) {
            return true;
        }

        protected virtual void LogCreation(T data) {
        }

    }
}
