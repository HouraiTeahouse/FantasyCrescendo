using UnityEngine;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(ParticleSystem))]
    public class AutoDestroyParticleSystem : MonoBehaviour {

        private ParticleSystem _particleSystem;

        void Awake() {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        void Update() {
            if(_particleSystem == null || !_particleSystem.IsAlive())
                Destroy(gameObject);
        }

    }


}
