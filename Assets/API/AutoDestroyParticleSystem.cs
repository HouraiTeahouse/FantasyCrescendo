using UnityEngine;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(ParticleSystem))]
    public class AutoDestroyParticleSystem : MonoBehaviour {

        private ParticleSystem particleSystem;

        void Awake() {
            particleSystem = GetComponent<ParticleSystem>();
        }
        void Update() {
            if(particleSystem == null || !particleSystem.IsAlive())
                Destroy(gameObject);
        }
    }


}
