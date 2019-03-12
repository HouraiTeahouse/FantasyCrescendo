using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/BGM")]
public class BGM : ScriptableObject {
  public AudioClipReference Clip;
  public string Author;
  public string Original;
  [Range(0f, 1f)] public float DefaultWeight;
}

}
