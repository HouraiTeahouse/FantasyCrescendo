using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Matches
{

public class MatchCountdownUI : MonoBehaviour
{
    // Component References
    // EditorBrowsable attribute hides the public variables from intellisense
    // In other words, you can still access it, but it won't be in the autocomplete list
    // If variables are null, look for them
    [Header("Component References")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TextMeshProUGUI _TextUI;
    public TextMeshProUGUI TextUI {
        get {
            if (!_TextUI)
                _TextUI = GetComponentInChildren<TextMeshProUGUI>();
            return _TextUI;
            }
        }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AudioSource _AudioPlayer;
    public AudioSource AudioPlayer
    {
        get
        {
            if (!_AudioPlayer)
                _AudioPlayer = GetComponentInChildren<AudioSource>();
            return _AudioPlayer;
        }
    }

    [Header("Timings")]
    public float InitialDelay = 1f;
    public float GoDuration = 1.25f;

    [Header("Countdown Clips (0 = 'GO', 1-10 = Time)")]
    public AudioClip[] CountdownClips;

    [Header("Debug")]
    public bool DisableCountdown = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Mediator.Global.CreateUnityContext(this).Subscribe<MatchStartCountdownEvent>(StartCountdown);
    }

    /// <summary>
    /// Starts countdown UI. Counts from given starting number to 1 to GO!
    /// Returns when GO! is reached, but GO! will continue to be displayed for a short duration
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    async Task StartCountdown(MatchStartCountdownEvent evt)
    {
        // For Debug purposes
        if (DisableCountdown) return;

        // Wait for initial delay
        await Task.Delay((int)(InitialDelay * 1000));

        // Enable Text GUI in case
        ObjectUtil.SetActive(TextUI.gameObject, true);

        // TODO: Make timer value based on evt
        var timer = 3;
        while (timer > 0)
        {
            await UpdateTextUI(timer.ToString(), CountdownClips[timer], 1);
            timer--;
        }
        // TODO: Make GO! string into localization string 
        UpdateTextUI("GO!", CountdownClips[0], GoDuration, true);
    }

    /// <summary>
    /// Updates Text GUI text based on input and plays AudioClip.
    /// Controls display time and whether it disappears afterwards as well
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="timer"></param>
    /// <param name="autoDisappear"></param>
    /// <returns></returns>
    async Task UpdateTextUI(string text, AudioClip audio, float timer, bool autoDisappear=false)
    {
        // Update Text and play Audio
        TextUI.text = text;
        AudioPlayer.clip = audio;
        AudioPlayer.Play();

        // Wait and maybe disable UI afterwards
        await Task.Delay((int)(timer * 1000));
        if (autoDisappear)
            ObjectUtil.SetActive(TextUI.gameObject, false);
    }
}

}