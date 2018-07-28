using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Matches
{

public class MatchCountdownUI : MonoBehaviour
{
  // Component References
  [Header("Component References")]
  public TextMeshProUGUI TextUI;
  public AudioSource AudioPlayer;

  [Header("Timings")]
  public float InitialDelay = 1f;

  [Header("Countdown Clips (0 = 'GO', 1-3 = Time)")]
  public AudioClip[] CountdownClips;

  [Header("Debug")]
  public bool DisableCountdown = false;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Mediator.Global.CreateUnityContext(this)
        .Subscribe<MatchStartCountdownEvent>(StartCountdown);
    TextUI = GetComponentInChildren<TextMeshProUGUI>();
    AudioPlayer = GetComponentInChildren<AudioSource>();
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

    await Task.Delay((int)(InitialDelay * 1000));
    ObjectUtil.SetActive(TextUI.gameObject, true);

    var timer = CountdownClips.Length - 1;
    while (timer > 0) {
      await UpdateTextUI(timer.ToString(), CountdownClips[timer]);
      timer--;
    }
    // TODO: Make GO! string into localization string 
    UpdateTextUIGO("GO!", CountdownClips[0]);
  }

  /// <summary>
  /// Updates Text GUI text based on input and plays AudioClip.
  /// Used for countdown for the await
  /// </summary>
  /// <param name="text"></param>
  /// <param name="audio"></param>
  /// <returns></returns>
  async Task UpdateTextUI(string text, AudioClip audio) {
    TextUI.text = text;
    AudioPlayer.clip = audio;
    AudioPlayer.Play();

    await Task.Delay((int)(audio.length * 1000));
  }

  /// <summary>
  /// Updates Text GUI text based on input and plays AudioClip.
  /// Used for go to remove the warning.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="audio"></param>
  async void UpdateTextUIGO(string text, AudioClip audio){
    await UpdateTextUI(text, audio);
    ObjectUtil.SetActive(TextUI.gameObject, false);
  }

    
}

}