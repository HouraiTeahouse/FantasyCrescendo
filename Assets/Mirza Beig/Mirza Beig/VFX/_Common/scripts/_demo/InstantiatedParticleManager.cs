
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections;

using System.Collections.Generic;

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

            public class InstantiatedParticleManager : ParticleManager
            {
                // =================================	
                // Nested classes and structures.
                // =================================

                // ...

                // =================================	
                // Variables.
                // =================================

                public LayerMask mouseRaycastLayerMask;
                List<ParticleSystems> spawnedPrefabs;

                // Don't allow spawning if true.
                // Used for button clicks vs. empty-space clicks.

                public bool disableSpawn { get; set; }

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

                    disableSpawn = false;
                    spawnedPrefabs = new List<ParticleSystems>();
                }

                // Get rid of spawned systems when re-activated.

                void OnEnable()
                {
                    //clear();
                }

                // ...

                public void clear()
                {
                    if (spawnedPrefabs != null)
                    {
                        for (int i = 0; i < spawnedPrefabs.Count; i++)
                        {
                            if (spawnedPrefabs[i])
                            {
                                Destroy(spawnedPrefabs[i].gameObject);
                            }
                        }

                        spawnedPrefabs.Clear();
                    }
                }

                // ...

                protected override void Update()
                {
                    base.Update();
                }

                // ...

                public void instantiateParticlePrefab(Vector3 mousePosition)
                {
                    if (!disableSpawn)
                    {
                        mousePosition.z = -Camera.main.transform.localPosition.z;
                        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                        ParticleSystems newParticlePrefab = Instantiate(
                            particlePrefabs[currentParticlePrefab], spawnPosition, Quaternion.identity) as ParticleSystems;

                        newParticlePrefab.gameObject.SetActive(true);

                        // Parent to spawner.

                        newParticlePrefab.transform.parent = transform;

                        spawnedPrefabs.Add(newParticlePrefab);
                    }
                }

                // ...

                public void instantiateParticlePrefabRandom()
                {
                    if (!disableSpawn)
                    {
                        instantiateParticlePrefab(new Vector3(
                            Random.Range(0.0f, Screen.width), Random.Range(0.0f, Screen.height), 0.0f));
                    }
                }

                // ...

                public void randomize()
                {
                    currentParticlePrefab = Random.Range(0, particlePrefabs.Count);
                }

                // Get particle count from all spawned.

                public override int getParticleCount()
                {
                    int pcount = 0;

                    for (int i = 0; i < spawnedPrefabs.Count; i++)
                    {
                        if (spawnedPrefabs[i])
                        {
                            pcount += spawnedPrefabs[i].getParticleCount();
                        }
                        else
                        {
                            spawnedPrefabs.RemoveAt(i);
                        }
                    }

                    return pcount;
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
