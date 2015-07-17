using UnityEngine;
using UnityEditor.Callbacks;
using Genso.API;

public class InjectStaticManagers : MonoBehaviour {

    private const string resourceLocation = "Prefabs/Static Managers";
    private const string targetTag = "Static Managers";

    [PostProcessScene(int.MaxValue)]
    public static void RemoveAllTestScripts() {

        GameObject staticManagers = GameObject.FindGameObjectWithTag(targetTag);
        if (staticManagers == null) {
            Debug.Log("Static Managers not found, injecting...");
            staticManagers = Resources.Load<GameObject>(resourceLocation).Copy();
        }

    }

}
