using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Image))]
    public class CharacterIconDisplay : MonoBehaviour, ICharacterGUIComponent {

        private Image Image;

        void Awake() {
            Image = GetComponent<Image>();
        }

        public void SetCharacterData(CharacterMatchData data) {
            if (Image == null)
                return;
            if (data == null || data.Data == null) {
                Image.enabled = false;
            } else {
                Image.enabled = true;
                Image.sprite = data.Data.Icon;
            }
        }

    }

}

