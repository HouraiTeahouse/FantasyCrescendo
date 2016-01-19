using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(RawImage))]
    public class CharacterImageUI : MonoBehaviour, ICharacterGUIComponent {

        private RawImage _image;

        [SerializeField]
        private bool _cropped;

        [SerializeField]
        private Color _disabledTint = Color.gray;

        [SerializeField]
        private Vector2 _rectBias;

        /// <summary>
        /// 
        /// </summary>
        void Awake() {
            _image = GetComponent<RawImage>();
        }

        public void SetCharacter(CharacterData character) {
            if (!character || character.AlternativeCount <= 0 || character.GetPortrait(0).Load() == null)
                return;
            _image.texture = character.GetPortrait(0).Asset.texture;
            Rect cropRect = _cropped ? character.CropRect : new Rect(0,0,1,1);

            cropRect.x += _rectBias.x;
            cropRect.y += _rectBias.y;
            _image.uvRect = cropRect;
            if (!character.IsSelectable)
                _image.color = _disabledTint;
        }

    }

}