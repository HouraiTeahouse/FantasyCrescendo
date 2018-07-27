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
        UpdateTextUI("GO!", CountdownClips[0], true);
    }

    /// <summary>
    /// Updates Text GUI text based on input and plays AudioClip.
    /// Controls display time and whether it disappears afterwards as well
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="timer"></param>
    /// <param name="autoDisappear"></param>
    /// <returns></returns>
    async Task UpdateTextUI(string text, AudioClip audio, bool autoDisappear=false) {
        // Update Text and play Audio
        TextUI.text = text;
        AudioPlayer.clip = audio;
        AudioPlayer.Play();

        // Wait and maybe disable UI afterwards
        await Task.Delay((int)(audio.length * 1000));
        if (autoDisappear)
            ObjectUtil.SetActive(TextUI.gameObject, false);
    }
}

}