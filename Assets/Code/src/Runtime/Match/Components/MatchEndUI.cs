using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchEndUI : MonoBehaviour {

  public Object GameEndedDisplay;
  public Object TimeUpDisplay;
  public float PostGameWait = 5f;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this).Subscribe<MatchEndEvent>(OnMatchEnd);
  }

  async Task OnMatchEnd(MatchEndEvent evt) {
    if (evt.MatchConfig.Time > 0 && evt.MatchState.Time <= 0) {
      ObjectUtil.SetActive(TimeUpDisplay, true);
    } else {
      ObjectUtil.SetActive(GameEndedDisplay, true);
    }
    await Task.Delay((int)(PostGameWait * 1000));
  }

}

}