using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Game Mode/Default Game Mode")]
public class DefaultGameMode : GameMode {
  public override AbstractMatch CreateMatch() => new DefaultMatch();
}

}

