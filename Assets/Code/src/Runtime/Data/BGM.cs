using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/BGM")]
public class BGM : ScriptableObject {
  [AssetReferenceTypeRestriction(typeof(AudioClip))] public AssetReference Clip;
  public string Author;
  public string Original;
  [Range(0f, 1f)] public float DefaultWeight;
}

}
