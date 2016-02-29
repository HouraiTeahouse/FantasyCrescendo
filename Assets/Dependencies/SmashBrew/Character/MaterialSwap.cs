using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// The pallete swap behaviour for changing out the 
    /// </summary>
    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public class MaterialSwap : MonoBehaviour {

        [Serializable]
        private class Swap {

            [Serializable]
            public class MaterialSet {

                [SerializeField, Resource(typeof (Material))]
                [Tooltip("The materials to apply to the renderers")]
                private string[] _materials;

                public void Set(Renderer[] targets) {
                    if (targets == null)
                        return;
                    Material[] loadedMaterials = new Material[_materials.Length];
                    for (var i = 0; i < loadedMaterials.Length; i++)
                        loadedMaterials[i] = Resources.Load<Material>(_materials[i]);
                    foreach(Renderer renderer in targets)
                        if(renderer)
                            renderer.sharedMaterials = loadedMaterials;
                }

            }

            [SerializeField, Tooltip("The set of materials to swap to")]
            private MaterialSet[] MaterialSets;

            [SerializeField, Tooltip("The set of renderers to apply the materials to")]
            private Renderer[] TargetRenderers;

            public void Set(int palleteSwap) {
                if (palleteSwap < 0 || palleteSwap >= MaterialSets.Length)
                    return;
                MaterialSets[palleteSwap].Set(TargetRenderers);
            }

            public int SetCount {
                get { return MaterialSets.Length; }
            }
        }

        [SerializeField]
        private Swap[] _swaps;

        private int _color;

        /// <summary>
        /// Gets the number of pallete swaps are available
        /// </summary>
        public int PalleteCount {
            get { return _swaps.Max(swap => swap.SetCount);  }
        }


        public int Pallete {
            get { return _color; }
            set {
                _color = value;
                foreach (Swap swap in _swaps)
                    swap.Set(value);
            }
        }

    }

} 
