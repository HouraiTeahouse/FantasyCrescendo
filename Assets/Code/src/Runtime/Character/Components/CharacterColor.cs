using HouraiTeahouse.Tasks;
using HouraiTeahouse.Loadables; 
using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

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

      public ITask Set(Renderer[] targets) {
        if (targets == null) {
          return Task.Resolved;
        }
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) {
          var materials = Materials.Select(path => Asset.Get<Material>(path).Load());
          ApplyMaterials(materials.ToArray(), targets);
          return Task.Resolved;
        }
#endif
        var materialTasks = Materials.Select(path => Asset.Get<Material>(path).LoadAsync());
        return Task.All(materialTasks).Then(materials => ApplyMaterials(materials, targets));
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

    public ITask Set(uint palleteSwap) {
      if (palleteSwap >= MaterialSets.Length) {
        return Task.Resolved;
      }
      return MaterialSets[palleteSwap].Set(TargetRenderers);
    }

  }

  public Swap[] Swaps;

  public int Count => Swaps?.Length > 0 ? Swaps?.Max(swap => swap.Count) ?? 0 : 0;

  public ITask Initialize(PlayerConfig config, bool isView) {
    if (!isView || Swaps == null) {
      return Task.Resolved;
    }
    return SetColor(config.Selection.Pallete);
  }

  public ITask SetColor(uint color) {
    return Task.All(Swaps.Select(swap => swap.Set(color)));
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
