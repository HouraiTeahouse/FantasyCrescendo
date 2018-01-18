using HouraiTeahouse.EditorAttributes;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Physics Config")]
public class PhysicsConfig : ScriptableObject {

  public LayerMask StageLayers;
  [Layer] public int HurtboxLayer;

  public int HurtboxLayerMask => 1 << HurtboxLayer;

}

}
