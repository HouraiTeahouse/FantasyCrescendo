using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Shield State")]
    public sealed class ShieldState : NetworkBehaviour, ICharacterState {

        // Character Constrants
        [Header("Constants")]
        [SerializeField]
        [Tooltip("Maximum shield health for the character. Remains constant.")]
        float _maxHealth = 100;

        [SerializeField]
        [Tooltip("How much shield health is lost over time. Remains constant")]
        float _depletionRate = 15;

        [SerializeField]
        [Tooltip("How fast shield health replenishes when not in use. Remains constant.")]
        float _regenRate = 25;

        // Character Variables 
        [Header("Variables")]
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("How much shield health the character currently has")]
        float _health;

        public float MaxHealth {
            get { return _maxHealth; }
        }

        public float DepletionRate {
            get { return _depletionRate; }
        }

        public float RegenRate {
            get { return _regenRate; }
        }

        public float Health {
            get { return _health; }
            set { _health = value; }
        }

        public void ResetState() { Health = MaxHealth; }

    }

}

