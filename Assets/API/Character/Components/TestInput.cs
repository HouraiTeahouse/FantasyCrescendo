using UnityEngine;
using System.Collections;

namespace Crescendo.API {


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
				return Movement.y < 0;
            }
        }

        public bool Attack {
            get { return Input.GetButtonDown(AttackButton); }
        }

        protected override void Start()
        {
            base.Start();
            InputSource = this;
        }

    }

}