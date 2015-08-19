using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class TestInput : CharacterComponent, ICharacterInput {

        private const string HorizontalAxis = "horizontal";
        private const string VerticalAxis = "vertical";
        private const string JumpAxis = "jump";
        private const string AttackButton = "attack";
        private const string SpecialButton = "special";
        private const string ShieldButton = "shield";

        public Vector2 Movement {
            get {
                int x = Util.Sign((int) Input.GetAxisRaw(HorizontalAxis));
                int y = Util.Sign((int) Input.GetAxisRaw(VerticalAxis));
                return new Vector2(x, y);
            }
        }

        public bool Jump {
            get { return Input.GetButtonDown(JumpAxis); }
        }

        public bool Crouch {
            get { return Movement.y < 0; }
        }

        public bool Attack {
            get { return Input.GetButtonDown(AttackButton); }
        }

        public bool Special {
            get { return Input.GetButtonDown(SpecialButton); }
        }

        public bool Shield {
            get { return Input.GetButton(ShieldButton); }
        }

        protected override void Start() {
            base.Start();
            InputSource = this;
        }

    }

}