
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections;

using UnityEngine.UI;

using MirzaBeig.Common;
using MirzaBeig.Animation;

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

            public class LoadingScreen : MonoBehaviour
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                // ...

                public Text loadingText;
                public string loadingTextStringFormat = "00.00%";

                public Image loadingImageFill;

                // Just for quick testing from the editor.
                // >> Turn OFF (set FALSE) for release builds.

                public bool simulateLoading = true;
                public Timer simulatedLoadingTimer = new Timer(0.0f, 10.0f);

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
                    if (!simulateLoading)
                    {
                        simulatedLoadingTimer.stop();
                    }

                    simulatedLoadingTimer.onTimerCompleteEvent += onLoadingComplete;
                }

                // ...

                void Update()
                {
                    float progress = simulateLoading ?

                        simulatedLoadingTimer.normalizedTime :
                        Application.GetStreamProgressForLevel(1);

                    // ...

                    loadingImageFill.fillAmount = progress;
                    loadingText.text = progress.ToString(loadingTextStringFormat);

                    if (Application.CanStreamedLevelBeLoaded(1) && !simulateLoading)
                    {
                        onLoadingComplete();
                    }

                    // ...

                    simulatedLoadingTimer.update();
                }

                // ...

                void onLoadingComplete()
                {
                    if (!simulateLoading || !simulatedLoadingTimer.loop)
                    {
                        loadingImageFill.fillAmount = 1.0f;
                        loadingText.text = "DONE! Loading Demo...";

                        Application.LoadLevel(1);
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
