using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Image))]
    public class IconDisplay : CharacterUIComponent<Image> {

        public override void SetCharacter(CharacterData data) {
            base.SetCharacter(data);
            if (Component == null || data == null)
                return;
            Component.sprite = data.Icon.Load();
        }

    }

}

