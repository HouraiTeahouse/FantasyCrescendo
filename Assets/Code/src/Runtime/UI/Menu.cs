using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
  
public class Menu : MonoBehaviour {

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => MenuManager.Instance.ForceNull()?.SetMenu(this);

}

}