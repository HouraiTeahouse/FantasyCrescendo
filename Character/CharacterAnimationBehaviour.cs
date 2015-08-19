using UnityEngine;

namespace Hourai.SmashBrew {

    public abstract class CharacterAnimationBehaviour : StateMachineBehaviour {

        private Character _character;

        protected virtual Character Character {
            get { return _character; }
            set { _character = value; }
        }

        internal void SetCharacter(Character character) {
            _character = character;
        }

    }

}