
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections;

using UnityEngine.UI;

using MirzaBeig.Common;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace VFX
    {

        namespace Demo
        {

            // =================================	
            // Classes.
            // =================================

            //[ExecuteInEditMode]
            [System.Serializable]

            //[RequireComponent(typeof(TrailRenderer))]

            public class MouseFollow : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                Vector3 position;
                public float speed = Mathf.Infinity;

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

                }

                // ...

                void Update()
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = -Camera.main.transform.localPosition.z;

                    position = Vector3.Lerp(
                        position, Camera.main.ScreenToWorldPoint(mousePosition), Time.deltaTime * speed);

                    transform.position = position;
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

}

// =================================	
// --END-- //
// =================================
