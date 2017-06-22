using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew.Characters {

    /// <summary> The pallete swap behaviour for changing out the </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Characters/Color State")]
    public class ColorState : CharacterComponent {

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
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying) {
                        var loadedMaterials = _materials.Select(path => Resource.Get<Material>(path).Load()).ToArray();
                        foreach (Renderer renderer in targets) {
                            if(renderer != null)
                                renderer.sharedMaterials = loadedMaterials;
                        }
                    } else
#endif
                    {
                        Task.All(_materials.Select(path => Resource.Get<Material>(path).LoadAsync()))
                            .Then(materials => {
                                foreach (Renderer renderer in targets) {
                                    if (renderer != null)
                                        renderer.sharedMaterials = materials;
                                }
                            });
                    }
                }

            }

            [SerializeField]
            [Tooltip("The set of materials to swap to")]
            MaterialSet[] MaterialSets;

            [SerializeField]
            [Tooltip("The set of renderers to apply the materials to")]
            Renderer[] TargetRenderers;

            /// <summary>
            /// The count of available swaps for this material swap set
            /// </summary>
            public int Count {
                get { return MaterialSets.Length; }
            }

            public void Set(int palleteSwap) {
                if (palleteSwap < 0 || palleteSwap >= MaterialSets.Length)
                    return;
                MaterialSets[palleteSwap].Set(TargetRenderers);
            }

            public void Reset() {
                foreach (Renderer renderer in TargetRenderers) {
                    if (renderer != null)
                        renderer.sharedMaterials = new Material[renderer.sharedMaterials.Length];
                }
            }

        }

        [SerializeField, ReadOnly]
        [SyncVar(hook = "ChangeColor")]
        int _color;

        [SerializeField]
        Swap[] _swaps;

        /// <summary> Gets the number of pallete swaps are available </summary>
        public int Count {
            get { return !_swaps.IsNullOrEmpty() ? _swaps.Max(s => s.Count) : 0; }
        }

        public int Pallete {
            get { return _color; }
            set { ChangeColor(value); }
        }

        public override void OnStartClient() {
            if(!isServer)
                ChangeColor(_color);
        }

        void ChangeColor(int color) {
            _color = color;
            if (_swaps == null)
                return;
            foreach (Swap swap in _swaps)
                swap.Set(color);
            Log.Info("Set color on {0} to {1}", gameObject.name, _color);
        }


#if UNITY_EDITOR
        // For editor only.
        public void Refesh(int value) {
            _color = value;
            foreach (Swap swap in _swaps)
                swap.Set(value);
        }

        public void ResetSwaps() {
            foreach (Swap swap in _swaps)
                swap.Reset();
        }
#endif

    }

}
