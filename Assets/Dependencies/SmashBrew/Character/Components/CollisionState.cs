using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Collision State")]
    public class CollisionState : CharacterComponent {

        [Header("Variables")]
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("Whether or not the character collides with attacks")]
        bool _intangible;

        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("Whether or not the character takes damage from attacks")]
        bool _invincible;

        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("Whether or not the character takes knockback from attacks")]
        bool _superArmor;

        //TODO(james7132): Consider changing this to a funtion
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("The base reduciton in damage from attacks")]
        float _damageReduction = 0;

        //TODO(james7132): Consider changing this to a funtion
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("The base reduciton in knockback from attacks")]
        float _launchResistance = 0;

        public bool Intangible {
            get { return _intangible; }
            set { _intangible = value; }
        }

        public bool Invincible {
            get { return _invincible; }
            set { _invincible = value; }
        }

        public bool Armor {
            get { return _superArmor; }
            set { _superArmor = value; }
        }

        public float DamageReduction {
            get { return _damageReduction; }
            set { _damageReduction = value; }
        }

        public float LaunchResistance {
            get { return _launchResistance; }
            set { _launchResistance = value; }
        }

    }

}
