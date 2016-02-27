using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    [RequireComponent(typeof(Image))]
    public class IconDisplay : CharacterUIComponent<Image> {

        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null)
                return;
            Component.sprite = data.Icon.Load();
        }

    }

}

