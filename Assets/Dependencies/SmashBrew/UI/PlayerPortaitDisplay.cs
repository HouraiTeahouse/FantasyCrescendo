using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class PlayerPortaitDisplay : MonoBehaviour, IPlayerGUIComponent {

        [SerializeField]
        private RawImage Image;

        private Player _player;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            if(!Image)
                Image = GetComponent<RawImage>();
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            if (_player != null)
                _player.OnChanged -= UpdateImage;
        }

        public void SetPlayerData(Player data) {
            if (data == null)
                return;
            if (_player != null)
                _player.OnChanged -= UpdateImage;
            _player = data;
            _player.OnChanged += UpdateImage;
            UpdateImage();
        }

        void UpdateImage() {
            if (Image == null)
                return;
            if (_player == null || _player.Character == null) {
                Image.enabled = false;
            } else {
                Image.enabled = true;
                Image.texture = _player.Character.GetPortrait(_player.Pallete).Load().texture;
                Image.uvRect = _player.Character.CropRect(Image.texture);
            }
        }

    }

}

