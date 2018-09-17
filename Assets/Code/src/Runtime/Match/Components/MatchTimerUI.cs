using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo {

public class MatchTimerUI : ViewFactory<MatchState, MatchConfig>, IStateView<MatchState> {

  public TMP_Text DisplayText;

  int? lastMinutes;
  int? lastSeconds;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (DisplayText != null) {
      DisplayText.enabled = false;
    }
  }

  public override Task<IStateView<MatchState>[]> CreateViews(MatchConfig config) {
    var showText = DisplayText != null && config.Time > 0;
    var views = DisplayText != null ? new IStateView<MatchState>[] {this} : new IStateView<MatchState>[0];
    if (DisplayText != null) {
      DisplayText.enabled = showText;
    }
    return Task.FromResult(views);
  }

  public void UpdateView(in MatchState state) {
    if (DisplayText == null) return;
    int seconds = Mathf.FloorToInt(state.Time * Time.fixedDeltaTime);
    int minutes = seconds / 60;
    seconds = seconds % 60;
    if (lastMinutes == minutes && lastSeconds == seconds) return;
    DisplayText.text = $"{minutes:00}:{seconds:00}";
    lastMinutes = minutes;
    lastSeconds = seconds;
  }

  public void Dispose() {
    ObjectUtil.Destroy(this);
    ObjectUtil.Destroy(DisplayText);
  }

}

}