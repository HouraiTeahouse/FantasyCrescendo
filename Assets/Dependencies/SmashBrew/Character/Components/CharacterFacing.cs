using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public class CharacterFacing : CharacterComponent {

        private bool _facing;

        [SerializeField]
        private FacingMode _facingMode;

        /// <summary>
        /// The direction the character is currently facing.
        /// If set to true, the character faces the right.
        /// If set to false, the character faces the left.
        /// 
        /// The method in which the character is flipped depends on what the Facing Mode parameter is set to.
        /// </summary>
        public bool Facing {
            get {
                if (_facingMode == FacingMode.Rotation)
                    return Character.eulerAngles.y > 179f;
                return Character.localScale.x > 0;
            }
            set {
                if (_facing == value)
                    return;

                _facing = value;
                if (_facingMode == FacingMode.Rotation)
                    Character.Rotate(0f, 180f, 0f);
                else {
                    Vector3 temp = Character.localScale;
                    temp.x *= -1;
                    Character.localScale = temp;
                }
            }
        }

        private void Update() {
            if (InputSource == null)
                return;

            Vector2 movement = InputSource.Movement;

            GetComponent<IDamageable>();

            //Ensure that the character is walking in the right direction
            if ((movement.x > 0 && Facing) ||
                (movement.x < 0 && !Facing))
                Facing = !Facing;
        }

        private enum FacingMode {

            Rotation,
            Scale

        }

    }

}