
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections;

using MirzaBeig.Common;

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
        [RequireComponent(typeof(ParticleSystem))]

        public class SpritesheetFrame : MonoBehaviour
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            // ...

            // =================================	
            // Variables.
            // =================================

            // ...

            public int columns = 8;
            public int rows = 8;

            //public int frame = 32;
            public MathUtility.RangeInt frameRange = new MathUtility.RangeInt(32, 48);

            new Renderer renderer;
            Material materialCopy;

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
                // Get material.

                renderer = GetComponent<Renderer>();

                materialCopy = new Material(renderer.sharedMaterial);
                renderer.sharedMaterial = materialCopy;

                Vector2 frameSize = new Vector2(1.0f / columns, 1.0f / rows);
                renderer.sharedMaterial.SetTextureScale("_MainTex", frameSize);

                setFrame();
            }

            // ...

            void setFrame()
            {

                // Set frame.

                int frame = frameRange.random;

                float frameX = (frame % rows) / (float)rows;
                float frameY = (frame / rows) / (float)columns;

                renderer.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(frameX, 1.0f - frameY));
            }

            // ...

            void Update()
            {

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
