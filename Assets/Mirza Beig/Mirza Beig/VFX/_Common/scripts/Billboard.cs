
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

        public class Billboard : MonoBehaviour
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            // ...

            // =================================	
            // Variables.
            // =================================

            // ...

            public bool billboardOnStart = true;
            public bool alwaysBillboard = false;

            public bool startRotationOffset = true;

            public bool billboardX = true;
            public bool billboardY = true;
            public bool billboardZ = true;

            // ...

            Quaternion startRotation;

            // Not sure if there's much of a use-case for allowing this to be public/editor-exposed.
            // It looks messy when public, but I could also just write an editor extension to hide/show it.

            Transform billboardTarget;

            // =================================	
            // Functions.
            // =================================

            // ...

            void Awake()
            {

            }

            // ...

            void Start()
            {
                startRotation = transform.localRotation;

                // ...

                if (!billboardTarget)
                {
                    billboardTarget = Camera.main.transform;
                }
                if (billboardOnStart)
                {
                    transform.LookAt(billboardTarget);

                    if (startRotationOffset)
                    {
                        transform.rotation *= startRotation;
                    }
                }
            }

            // ...

            void onParticleSystemsDead()
            {
                Destroy(gameObject);
            }

            // ...

            void Update()
            {

            }

            // ...

            void LateUpdate()
            {
                if (alwaysBillboard)
                {
                    Vector3 targetPosition = billboardTarget.position;

                    if (!billboardX)
                    {
                        targetPosition.x = transform.position.x;
                    }
                    if (!billboardY)
                    {
                        targetPosition.y = transform.position.y;
                    }
                    if (!billboardZ)
                    {
                        targetPosition.z = transform.position.z;
                    }

                    transform.LookAt(billboardTarget);

                    if (startRotationOffset)
                    {
                        transform.rotation *= startRotation;
                    }
                }
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
