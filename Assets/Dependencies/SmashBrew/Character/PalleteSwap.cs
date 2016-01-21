using System.Collections.Generic;
using UnityEngine;

namespace Hourai.SmashBrew {

    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public class PalleteSwap : MonoBehaviour {

        [System.Serializable]
        private class Swap {

            [System.Serializable]
            public class MaterialSet {

                [SerializeField, Resource(typeof (Material))]
                private string[] _materials;

                public void Set(IEnumerable<Renderer> targets) {
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

            [SerializeField]
            private MaterialSet[] MaterialSets;

            [SerializeField]
            private Renderer[] TargetRenderers;

            public void Set(int palleteSwap) {
                if (palleteSwap < 0 || palleteSwap >= MaterialSets.Length)
                    return;
                MaterialSets[palleteSwap].Set(TargetRenderers);
            }

        }

        [SerializeField]
        private Swap[] _swaps;

        void Start() {
            var playerController = GetComponentInParent<PlayerController>();
            if (playerController.PlayerData == null)
                Debug.LogError("PlayerController does not have a Player", playerController);
            else { 
                int palletSwap = playerController.PlayerData.Pallete;
                foreach (Swap swap in _swaps) 
                    swap.Set(palletSwap);
            }
        }

    }

} 
