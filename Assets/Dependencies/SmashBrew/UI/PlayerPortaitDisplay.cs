using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(RawImage))]
    public class PlayerPortaitDisplay : MonoBehaviour, IPlayerGUIComponent {

        private RawImage Image;

        void Awake() {
            Image = GetComponent<RawImage>();
        }

        public void SetPlayerData(Player data)
        {
            if (Image == null)
                return;
            if (data == null || data.Character == null) {
                Image.enabled = false;
            } else {
                Image.enabled = true;
                Image.texture = data.Character.GetPortrait(data.Pallete).Load().texture;
                Image.uvRect = data.Character.CropRect;
            }
        }

    }

}

