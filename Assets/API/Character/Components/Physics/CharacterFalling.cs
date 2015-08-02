using UnityEngine;
using System.Collections;
using Crescendo.API;

namespace Genso.API {


    public class CharacterFalling : CharacterComponent
    {

        [SerializeField]
        private float _maxFallSpeed = 5f;

        [SerializeField]
        private float _fastFallSpeed = 9f;

        [SerializeField]
        private float _helplessFallSpeed = 9f;

        private float FallSpeed
        {
            get
            {
                if (Character.IsHelpless)
                    return _helplessFallSpeed;
                if (Character.IsFastFalling)
                    return _fastFallSpeed;
                return _maxFallSpeed;
            }
        }

        void FixedUpdate() {
            Vector3 velocity = Character.Velocity;
            
            if (Character.IsFastFalling || velocity.y < -FallSpeed)
                velocity.y = -FallSpeed;

            Character.Velocity = velocity;
        }


    }

}