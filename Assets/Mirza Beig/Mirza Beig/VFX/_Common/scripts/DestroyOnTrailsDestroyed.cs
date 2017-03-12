
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

// =================================	
// Classes.
// =================================

//[ExecuteInEditMode]
[System.Serializable]

public class DestroyOnTrailsDestroyed : MonoBehaviour
{
    // =================================	
    // Nested classes and structures.
    // =================================

    // ...

    // =================================	
    // Variables.
    // =================================

    // If true, keep checking children for trails.
    // Else, get the list once and check if entire list is null.

    public bool continousRefresh = false;
    TrailRenderer[] trails;

    // =================================	
    // Functions.
    // =================================

    // ...

    void Awake()
    {
        trails = GetComponentsInChildren<TrailRenderer>();
    }

    // ...

    void Start()
    {

    }

    // ...

    void Update()
    {
        if (!continousRefresh)
        {
            bool destroy = true;

            for (int i = 0; i < trails.Length; i++)
            {
                if (trails[i] != null)
                {
                    destroy = false; break;
                }
            }

            if (destroy)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (GetComponentsInChildren<TrailRenderer>().Length == 0)
            {
                Destroy(gameObject);
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

// =================================	
// --END-- //
// =================================
