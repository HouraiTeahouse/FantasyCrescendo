using UnityEngine;

namespace Hourai.SmashBrew {
    
    public enum Direction {
        Neutral = 0,
        Forward,
        Backward,
        Up,
        Down
    }

    public enum AttackType {
        Ground = 0,
        Smash,
        Special,
        Aerial
    }

    public class Attack : CharacterAnimationBehaviour {

        [SerializeField]
        private Direction _direction;

        [SerializeField]
        private AttackType _type;

        public Direction Direction {
            get { return _direction; }
        }

        public AttackType Type {
            get { return _type; }
        }

    }

}