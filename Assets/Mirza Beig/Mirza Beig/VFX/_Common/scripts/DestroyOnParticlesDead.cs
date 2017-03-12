
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace VFX
    {

        // =================================	
        // Classes.
        // =================================

        //[ExecuteInEditMode]
        [System.Serializable]

        public class DestroyOnParticlesDead : ParticleSystems
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            // ...

            // =================================	
            // Variables.
            // =================================

            // ...

            // =================================	
            // Functions.
            // =================================

            // ...

            protected override void Awake()
            {
                base.Awake();
            }

            // ...

            protected override void Start()
            {
                base.Start();

                // ...

                onParticleSystemsDeadEvent += onParticleSystemsDead;
            }

            // ...

            void onParticleSystemsDead()
            {
                Destroy(gameObject);
            }

            // ...

            protected override void Update()
            {
                base.Update();

                //for (int i = 0; i < particleSystems.Length; i++)
                //{
                //    //print(particleSystems[i].)
                //}
            }

            // ...

            protected override void LateUpdate()
            {
                base.LateUpdate();
            }

            // =================================	
            // End functions.
            // =================================

        }

        // =================================	
        // End namespace.
        // =================================

    }

}

// =================================	
// --END-- //
// =================================
