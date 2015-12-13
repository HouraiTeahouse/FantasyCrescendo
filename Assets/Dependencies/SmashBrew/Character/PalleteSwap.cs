using UnityEngine;

namespace Hourai.SmashBrew {

    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public class PalleteSwap : MonoBehaviour {

        [System.Serializable]
        private class Swap {

            [Resource(typeof(Material))]
            public string[] AlternativeMaterials;
            public Renderer[] TargetRenderers;

        }

        [SerializeField]
        private Swap[] _swaps;

        void Start() {
            var playerController = GetComponentInParent<PlayerController>();
            if (playerController.PlayerData == null)
                Debug.LogError("PlayerController does not have a Player", playerController);
            else { 
                int palletSwap = playerController.PlayerData.Pallete;
                foreach (var swap in _swaps) {
                    if (palletSwap < 0 || palletSwap >= swap.AlternativeMaterials.Length)
                        continue;
                    var material = Resources.Load<Material>(swap.AlternativeMaterials[palletSwap]);
                    if (material == null)
                        continue;
                    foreach(Renderer rend in swap.TargetRenderers)
                        if (rend)
                            rend.sharedMaterial = material;
                }
            }
        }

    }

} 
