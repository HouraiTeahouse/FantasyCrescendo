using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.UI;
using UnityEngine;

namespace HouraiTeahouse {

    [RequireComponent(typeof (CreateObject))]
    public class PlayerCreateObject : PlayerUIComponent {

        private CreateObject createObj;

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            createObj = GetComponent<CreateObject>();
            if (createObj == null) {
                Destroy(this);
                return;
            }
            createObj.OnCreate += OnCreate;
        }

        /// <summary>
        /// Unity callbakc. Called on object destruction.
        /// </summary>
        protected override void OnDestroy() {
            base.OnDestroy();
            if (createObj)
                createObj.OnCreate -= OnCreate;
        }

        void OnCreate(Object obj) {
            GameObject go = obj.GetGameObject();
            if (go == null)
                return;
            go.GetComponent<IDataComponent<Player>>().SetData(Player);
        }

    }

}
