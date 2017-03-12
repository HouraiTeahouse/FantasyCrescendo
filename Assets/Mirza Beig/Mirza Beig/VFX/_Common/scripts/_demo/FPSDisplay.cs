
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

            [ExecuteInEditMode]
            [System.Serializable]

            public class FPSDisplay : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // Singleton.

                static FPSDisplay instance;

                // Display.

                Text fpsText;

                // Count and timer.

                int frames = 0;
                float time = 0.0f;

                public int targetFrameRate = 60;

                // Length in seconds of time "buffer" to average.

                public float updateTime = 1.0f;

                // String format.

                public string textFormat = "FPS (X/Xs-AVG): 00.00";

                // =================================	
                // Functions.
                // =================================

                // ...

                void Awake()
                {
                    if (instance)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        instance = this;
                    }

                    Application.targetFrameRate = targetFrameRate;
                }

                // ...

                void Start()
                {
                    fpsText = GetComponent<Text>();
                }

                // ...

                void Update()
                {
                    time += Time.deltaTime;

                    frames++;

                    if (time > updateTime)
                    {
                        fpsText.text = (1.0f / (time / frames)).ToString(textFormat);

                        time = 0.0f;
                        frames = 0;
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

}

// =================================	
// --END-- //
// =================================
