using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchEndUI : MonoBehaviour {

    // Component References
    // If variables are null, look for them
    [Header("Component References")]
    [SerializeField] private TextMeshProUGUI _textUI;
    public TextMeshProUGUI TextUI
    {
        get
        {
            if (_textUI == null)
                _textUI = GetComponentInChildren<TextMeshProUGUI>();
            return _textUI;
        }
    }
    [SerializeField] private AudioSource _audioPlayer;
    public AudioSource AudioPlayer
    {
        get
        {
            if (_audioPlayer == null)
                _audioPlayer = GetComponentInChildren<AudioSource>();
            return _audioPlayer;
        }
    }
    [Header("Timings")]
    public float PostGameWait = 5f;

    [Header("Audio")]
    public AudioClip TimeAudioClip;
    public AudioClip GameAudioClip;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake() {
        Mediator.Global.CreateUnityContext(this).Subscribe<MatchEndEvent>(OnMatchEnd);
    }

    
    async Task OnMatchEnd(MatchEndEvent evt) {
        ObjectUtil.SetActive(TextUI.gameObject, true);
        if (evt.MatchConfig.Time > 0 && evt.MatchState.Time <= 0) {
            UpdateTextUI("TIME!", TimeAudioClip);
        }
        else {
            UpdateTextUI("GAME!", GameAudioClip);
        }
        await Task.Delay((int)(PostGameWait * 1000));
    }

    void UpdateTextUI(string text, AudioClip audio)
    {
        // Update Text and play Audio
        TextUI.text = text;
        AudioPlayer.clip = audio;
        AudioPlayer.Play();
    }

}

}