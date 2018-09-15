using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchCountdownUI : MonoBehaviour {

  // Component References
  [Header("Component References")]
  public TextMeshProUGUI TextUI;
  public AudioSource AudioPlayer;

  [Header("Timings")]
  public float InitialDelay = 1f;
  public float MinimumDelay = 1f;

  [Header("Countdown Clips (0 = 'GO', 1-3 = Time)")]
  public MatchStateDisplayData[] CountdownClips;

  [Header("Debug")]
  public bool DisableCountdown = false;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this)
        .SubscribeAsync<MatchStartCountdownEvent>(StartCountdown);
    if (TextUI == null) {
      TextUI = GetComponentInChildren<TextMeshProUGUI>();
    }
    if (AudioPlayer == null) {
      AudioPlayer = GetComponentInChildren<AudioSource>();
    }
  }

  /// <summary>
  /// Starts countdown UI. Based on Stage GUI's CountdownClips array.
  /// Each Countdown Audio Clip's length influences how long the text is displayed
  /// Returns when GO! is reached, but GO! will continue to be displayed for its duration
  /// </summary>
  /// <param name="evt"></param>
  /// <returns></returns>
  async Task StartCountdown(MatchStartCountdownEvent evt) {
    // For Debug purposes and you don't want to wait
    if (DisableCountdown && Debug.isDebugBuild) return;

    if (InitialDelay > 0) {
      await Task.Delay((int)(InitialDelay * 1000));
    }
    ObjectUtil.SetActive(TextUI.gameObject, true);

    for (var i = 0; i < CountdownClips.Length; i++) {
      if (i < CountdownClips.Length -1) {
        await UpdateTextUI(CountdownClips[i]);
      } else {
        UpdateTextUIGO(CountdownClips[i]);
      }
    }
  }

  /// <summary>
  /// Updates Text GUI text based on input and plays AudioClip.
  /// Used for countdown for the await
  /// </summary>
  /// <param name="text"></param>
  /// <param name="audio"></param>
  /// <returns></returns>
  async Task UpdateTextUI(MatchStateDisplayData data) {
    // Update Text and play Audio
    data.Apply(AudioPlayer, TextUI);

    float delay = MinimumDelay;
    if (data.Clip != null) {
      delay = Mathf.Min(delay, data.Clip.length);
    }

    // Wait and maybe disable UI afterwards
    await Task.Delay((int)(delay * 1000));
  }

  /// <summary>
  /// Updates Text GUI text based on input and plays AudioClip.
  /// Used for go to remove the warning.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="audio"></param>
  async void UpdateTextUIGO(MatchStateDisplayData data){
    await UpdateTextUI(data);
    ObjectUtil.SetActive(TextUI.gameObject, false);
  }

}

}