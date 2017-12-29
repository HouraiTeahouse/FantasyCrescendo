using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct PlayerInput {

  // One Player Total: 17 bytes
  // Four Player Total: 68 bytes
  //
  // 60 times one: 1020 bytes
  // 60 times four: 4080 bytes

  public bool IsValid;                          // 1 bit

  public Vector2 Movement;                      // 8 bytes
  public Vector2 Smash;                         // 8 bytes

  public bool Attack;                           // 1 bit
  public bool Special;                          // 1 bit
  public bool Jump;                             // 1 bit
  public bool Shield;                           // 1 bit
  public bool Grab;                             // 1 bit


}

}
