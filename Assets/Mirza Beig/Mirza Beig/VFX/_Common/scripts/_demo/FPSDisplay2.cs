
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

            // Adapted from Unity's sample.

            public class FPSDisplay2 : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // Singleton.

                static FPSDisplay2 instance;

                public float fpsMeasurePeriod = 0.5f;

                int m_FpsAccumulator = 0;
                float m_FpsNextPeriod = 0;

                int m_CurrentFps;

                // Display.

                Text fpsText;

                // String format.

                public string textFormat = "FPS (X/Xs-AVG): 00.00";

                // =================================	
                // Functions.
                // =================================

                // ...

                void Awake()
                {
                    if (instance && !Application.isEditor)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        instance = this;
                    }
                }

                // ...

                void Start()
                {
                    fpsText = GetComponent<Text>();
                    m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
                }

                // ...

                void Update()
                {
                    m_FpsAccumulator++;

                    if (Time.realtimeSinceStartup > m_FpsNextPeriod)
                    {
                        m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);

                        m_FpsAccumulator = 0;
                        m_FpsNextPeriod += fpsMeasurePeriod;

                        fpsText.text = m_CurrentFps.ToString(textFormat);
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

