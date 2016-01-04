using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Image))]
    public class PlayerPortaitDisplay : MonoBehaviour, IPlayerGUIComponent {

        private Image Image;

        void Awake() {
            Image = GetComponent<Image>();
        }

        public void SetPlayerData(Player data)
        {
            if (Image == null)
                return;
            if (data == null || data.Character == null) {
                Image.enabled = false;
            } else {
                Image.enabled = true;
                Image.sprite = data.Character.GetPortrait(data.Pallete).Load();
            }
        }

    }

}

