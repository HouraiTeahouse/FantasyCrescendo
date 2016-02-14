using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Image))]
    public class PlayerIconDisplay : MonoBehaviour, IPlayerGUIComponent {

        private Image Image;

        void Awake() {
            Image = GetComponent<Image>();
        }

        public void SetPlayerData(Player data) {
            if (Image == null)
                return;
            if (data == null || data.SelectedCharacter == null) {
                Image.enabled = false;
            } else {
                Image.enabled = true;
                Image.sprite = data.SelectedCharacter.Icon.Load();
            }
        }

    }

}

