// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> The pallete swap behaviour for changing out the </summary>
    [Required]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public class MaterialSwap : MonoBehaviour {
        [Serializable]
        public class Swap {
            [Serializable]
            public class MaterialSet {
                [SerializeField]
                [Resource(typeof(Material))]
                [Tooltip("The materials to apply to the renderers")]
                string[] _materials;

                public void Set(Renderer[] targets) {
                    if (targets == null)
                        return;
                    var loadedMaterials = new Material[_materials.Length];
                    for (var i = 0; i < loadedMaterials.Length; i++)
                        loadedMaterials[i] =
                            Resources.Load<Material>(_materials[i]);
                    foreach (Renderer renderer in targets)
                        if (renderer)
                            renderer.sharedMaterials = loadedMaterials;
                }
            }

            [SerializeField]
            [Tooltip("The set of materials to swap to")]
            MaterialSet[] MaterialSets;

            [SerializeField]
            [Tooltip("The set of renderers to apply the materials to")]
            Renderer[] TargetRenderers;

            public int SetCount {
                get { return MaterialSets.Length; }
            }

            public void Set(int palleteSwap) {
                if (palleteSwap < 0 || palleteSwap >= MaterialSets.Length)
                    return;
                MaterialSets[palleteSwap].Set(TargetRenderers);
            }
        }

        int _color;

        [SerializeField]
        Swap[] _swaps;

        /// <summary> Gets the number of pallete swaps are available </summary>
        public int PalleteCount {
            get { return _swaps.Max(swap => swap.SetCount); }
        }

        public int Pallete {
            get { return _color; }
            set {
                _color = value;
                foreach (Swap swap in _swaps)
                    swap.Set(value);
            }
        }

#if UNITY_EDITOR

        // For editor only. To refresh when
        public void Refesh(int value) {
            _color = value;
            foreach (Swap swap in _swaps)
                swap.Set(value);
        }

#endif
    }
}
