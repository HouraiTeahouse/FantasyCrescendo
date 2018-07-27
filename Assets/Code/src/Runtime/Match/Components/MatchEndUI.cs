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
    public AudioClip TimeAudioClip;
    public AudioClip GameAudioClip;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake() {
        Mediator.Global.CreateUnityContext(this)
            .Subscribe<MatchEndEvent>(OnMatchEnd);
        TextUI = GetComponentInChildren<TextMeshProUGUI>();
        AudioPlayer = GetComponentInChildren<AudioSource>();
    }

    
    async Task OnMatchEnd(MatchEndEvent evt) {
        ObjectUtil.SetActive(TextUI.gameObject, true);
        if (evt.MatchConfig.Time > 0 && evt.MatchState.Time <= 0) {
            UpdateTextUI("TIME!", TimeAudioClip);
        } else {
            UpdateTextUI("GAME!", GameAudioClip);
        }
        await Task.Delay((int)(PostGameWait * 1000));
    }

    void UpdateTextUI(string text, AudioClip audio) {
        // Update Text and play Audio
        TextUI.text = text;
        AudioPlayer.clip = audio;
        AudioPlayer.Play();
    }

}

}