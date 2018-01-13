using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Modes/Training Game Mode")]
public class TrainingGameMode : GameMode {

  public override AbstractMatch CreateMatch() => new DefaultMatch();

}

}

