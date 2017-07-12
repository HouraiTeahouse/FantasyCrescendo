using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {

    public class SceneSelector : SceneUIComponent, IPlayerClickable, IPointerClickHandler {

        void Load() {
            if (Scene)
                Scene.Load();
        }

        public void Click(Player player) { Load(); }

        void IPointerClickHandler.OnPointerClick(PointerEventData data) {
            Load();
        }

    }

}