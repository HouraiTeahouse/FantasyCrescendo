using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Input Config")]
public class InputConfig : ScriptableObject {

  [Range(0f, 1f)] public float DeadZone = 0.3f;
  [Range(0f, 1f)] public float SmashThreshold = 0.7f;
  public uint SmashFrameWindow = 3;

}
    
}
