using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchEndUI : MonoBehaviour {

  // Component References
  [Header("Component References")]
  public TextMeshProUGUI TextUI;
  public AudioSource AudioPlayer;

  [Header("Timings")]
  public float PostGameWait = 5f;

  [Header("Audio")]
  public MatchStateDisplayData Time;
  public MatchStateDisplayData Game;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this)
        .Subscribe<MatchEndEvent>(OnMatchEnd);
    if (TextUI == null) {
      TextUI = GetComponentInChildren<TextMeshProUGUI>();
    }
    if (AudioPlayer == null) {
      AudioPlayer = GetComponentInChildren<AudioSource>();
    }
  }
  
  async Task OnMatchEnd(MatchEndEvent evt) {
    ObjectUtil.SetActive(TextUI.gameObject, true);
    if (evt.MatchConfig.Time > 0 && evt.MatchState.Time <= 0) {
      Time.Apply(AudioPlayer, TextUI);
    } else {
      Game.Apply(AudioPlayer, TextUI);
    }
    await Task.Delay((int)(PostGameWait * 1000));
  }

}

}