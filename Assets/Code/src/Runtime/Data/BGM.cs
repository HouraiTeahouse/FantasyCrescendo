using HouraiTeahouse.Loadables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/BGM")]
public class BGM : ScriptableObject {
  [SerializeField, Resource(typeof(AudioClip))]
  string _audioClip;
  public IAsset<AudioClip> Clip => Asset.Get<AudioClip>(_audioClip);
  public string Author;
  public string Original;
  [Range(0f, 1f)] public float DefaultWeight;
}

}
