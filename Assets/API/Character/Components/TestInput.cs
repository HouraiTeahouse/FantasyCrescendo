using UnityEngine;
using System.Collections;

namespace Genso.API {


    public class TestInput : CharacterComponent, ICharacterInput
    {

        private const string HorizontalAxis = "horizontal";
        private const string VerticalAxis = "vertical";
        private const string JumpAxis = "jump";
        private const string AttackButton = "attack";

        public Vector2 Movement
        {
            get
            {
                int x = Util.Sign((int)Input.GetAxisRaw(HorizontalAxis));
                int y = Util.Sign((int)Input.GetAxisRaw(VerticalAxis));
                return new Vector2(x, y);
            }
        }

        public bool Jump
        {
            get { return Input.GetButtonDown(JumpAxis); }
        }

        public bool Crouch
        {
            get
            {
                // TODO: Implement properly with animations
                return false;
            }
        }

        public bool Attack {
            get { return Input.GetButtonDown(AttackButton); }
        }

        protected override void Awake()
        {
            base.Awake();
            InputSource = this;
        }

    }

}