using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class PhysicsState : NetworkBehaviour, ICharacterState {

        // Character Constrants
        [Header("Constants")]
        [SerializeField]
        [Tooltip("How much the character weighs")]
        float _weight = 1.0f;

        [SerializeField]
        [Tooltip("How fast a charactter reaches their max fall speed, in seconds.")]
        float _gravity = 1.5f;

        [SerializeField]
        [Tooltip("The maximum downward velocity of the character under normal conditions")]
        float _maxFallSpeed = 5f;

        [SerializeField]
        [Tooltip("The downward velocity of the character while fast falling")]
        float _fastFallSpeed = 5f;

        // Character Variables 
        [Header("Variables")]
        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("How much shield health the character currently has")]
        Vector2 _velocity;

        [SyncVar, SerializeField, ReadOnly]
        [Tooltip("How much shield health the character currently has")]
        Vector2 _acceleration;

        public void ResetState() { throw new System.NotImplementedException(); }

    }

}

