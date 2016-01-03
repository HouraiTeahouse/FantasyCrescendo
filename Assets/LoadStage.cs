using UnityEngine;
using UnityEngine.EventSystems;

namespace Hourai.SmashBrew.UI {

    public class LoadStage : MonoBehaviour, ISubmitHandler {

        [SerializeField]
        private SceneData _stage;

        /// <summary>
        /// The SceneData describing the Stage to load upon submit being entered.
        /// </summary>
        public SceneData Stage {
            get { return _stage; }
            set { _stage = value; }
        }

        public void OnSubmit(BaseEventData eventData) {
            if(_stage)
                _stage.Load();
            else
                Debug.LogWarning("Tried to load stage, but none found.", this);
        }

    }

}