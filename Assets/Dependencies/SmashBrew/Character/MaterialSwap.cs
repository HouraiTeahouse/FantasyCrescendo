using UnityEngine;

namespace Hourai.SmashBrew {

    [RequireComponent(typeof(Renderer))]
    public class MaterialSwap : MonoBehaviour {

        [SerializeField, Resource(typeof(Material))]
        private string[] _alternativeMaterials;

        void Start() {
            var playerController = GetComponentInParent<PlayerController>();
            if (playerController != null) {
                if (playerController.PlayerData == null)
                    Debug.LogError("PlayerController does not have a Player", playerController);
                else {
                    int palletSwap = playerController.PlayerData.Pallete;
                    if(palletSwap >= 0 && palletSwap < _alternativeMaterials.Length)
                        GetComponent<Renderer>().material = Resources.Load<Material>(_alternativeMaterials[palletSwap]);
                }
            }
            Destroy(this);
        }

    }

} 
