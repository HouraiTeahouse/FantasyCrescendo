
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
        [RequireComponent(typeof(Light))]

        public class AnimatedLight : MonoBehaviour
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            // ...

            // =================================	
            // Variables.
            // =================================

            // ...

            new Light light;

            public float time { get; set; }
            public float duration = 1.0f;

            bool evaluating = true;

            public Gradient colourOverLifetime;

            public AnimationCurve intensityOverLifetime = new AnimationCurve(

                new Keyframe(0.0f, 0.0f),
                new Keyframe(0.5f, 1.0f),
                new Keyframe(1.0f, 0.0f));

            // ...

            public bool loop = true;
            public bool autoDestruct = false;

            // ...

            Color startColour;
            float startIntensity;

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
                light = GetComponent<Light>();

                // ...

                startColour = light.color;
                startIntensity = light.intensity;

                light.color = startColour * colourOverLifetime.Evaluate(0.0f);
                light.intensity = startIntensity * intensityOverLifetime.Evaluate(0.0f);
            }

            // ...

            void Update()
            {
                if (evaluating)
                {
                    if (time < duration)
                    {
                        time += Time.deltaTime;

                        if (time > duration)
                        {
                            if (autoDestruct)
                            {
                                Destroy(gameObject);
                            }
                            else if (loop)
                            {
                                time = 0.0f;
                            }
                            else
                            {
                                time = duration;
                                evaluating = false;
                            }
                        }
                    }

                    // ...

                    if (time <= duration)
                    {
                        float normalizedTime = time / duration;

                        light.color = startColour * colourOverLifetime.Evaluate(normalizedTime);
                        light.intensity = startIntensity * intensityOverLifetime.Evaluate(normalizedTime);
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
