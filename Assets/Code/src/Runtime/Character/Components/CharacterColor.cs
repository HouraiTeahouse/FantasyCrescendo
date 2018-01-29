using HouraiTeahouse.FantasyCrescendo.Players;
using HouraiTeahouse.Loadables; 
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterColor : MonoBehaviour, IPlayerComponent {

  [HideInInspector, SerializeField]
  Material DefaultMaterial;

  [Serializable]
  public class Swap {

    [Serializable]
    public class MaterialSet {

      [Resource(typeof(Material))]
      [Tooltip("The materials to apply to the renderers")]
      public string[] Materials;

      public async Task Set(Renderer[] targets) {
        if (targets == null) {
          return;
        }
        Material[] materials;
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) {
          materials = Materials.Select(path => Asset.Get<Material>(path).Load()).ToArray();
          ApplyMaterials(materials, targets);
          return;
        }
#endif
        var materialTasks = Materials.Select(path => Asset.Get<Material>(path).LoadAsync());
        materials = await Task.WhenAll(materialTasks);
        ApplyMaterials(materials, targets);
      }

      void ApplyMaterials(Material[] materials, Renderer[] targets) {
        foreach (Renderer renderer in targets) {
          if (renderer != null) {
            renderer.sharedMaterials = materials;
          }
        }
      }
    }

    [Tooltip("The set of materials to swap to")]
    public MaterialSet[] MaterialSets;

    [Tooltip("The set of renderers to apply the materials to")]
    public Renderer[] TargetRenderers;

    /// <summary>
    /// The count of available swaps for this material swap set.
    /// </summary>
    public int Count => MaterialSets?.Length ?? 0;

    public async Task Set(uint palleteSwap) {
      if (palleteSwap >= MaterialSets.Length) {
        return;
      }
      await MaterialSets[palleteSwap].Set(TargetRenderers);
    }

  }

  public Swap[] Swaps;

  public int Count => Swaps?.Length > 0 ? Swaps?.Max(swap => swap.Count) ?? 0 : 0;

  public async Task Initialize(PlayerConfig config, bool isView) {
    if (!isView || Swaps == null) {
      return;
    }
    await SetColor(config.Selection.Pallete);
  }

  public async Task SetColor(uint color) {
    await Task.WhenAll(Swaps.Select(swap => swap.Set(color)));
  }

  /// <summary>
  /// Clears all Renderers to avoid them referencing the targett.
  /// </summary>
  public void Clear() {
    var renderers = Swaps.SelectMany(s => s.TargetRenderers).Distinct();
    foreach (var renderer in renderers) {
      if (renderer == null) continue;
      var materials = renderer.sharedMaterials;
      for (int i = 0; i < materials.Length; i++) {
        materials[i] = DefaultMaterial;
      }
      renderer.sharedMaterials = materials;
    }
  }

}

}
