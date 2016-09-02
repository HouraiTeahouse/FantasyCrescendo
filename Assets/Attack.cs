using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public enum AttackType {

        Normal,
        Tilt,
        Smash,
        Aerial,
        Special

    }

    public enum Direction {

        Neutral = 0,
        Up = 1,
        Down = -1,
        Forward = 2,
        Backward = -2

    }

    public class Attack : CharacterState<Attack> {

        [SerializeField]
        AttackType _type;

        [SerializeField]
        Direction _direction;

        public AttackType Type {
            get { return _type; }
            set { _type = value; }
        }

        public Direction Direction {
            get { return _direction; }
            set { _direction = value; }
        }

    }

    public static class DirectionUtil {

        public static Direction Opposite(this Direction direction) { return (Direction) ((int) direction * -1); }

        public static Vector2 ToVector(this Direction direction) {
            var dir = (int) direction;
            return new Vector2(dir / 2, dir % 2);
        }

    }

}