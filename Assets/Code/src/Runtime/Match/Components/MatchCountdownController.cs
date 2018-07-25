using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.Matches
{

public class MatchCountdownController : MonoBehaviour
{
    public Text TextUI;
    public float GoDuration = 1.25f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Mediator.Global.CreateUnityContext(this).Subscribe<MatchStartCountdownEvent>(StartCooldown);
    }

    async Task StartCooldown(MatchStartCountdownEvent evt)
    {
        // TODO: Make timer value based on evt
        var timer = 3;
        while (timer > 0)
        {
            await UpdateTextUI(timer.ToString(), 1);
            timer--;
        }
        // TODO: Make GO! string into localization string 
        UpdateTextUI("GO!", GoDuration, true);
    }

    async Task UpdateTextUI(string _text, float timer, bool autoDisappear=false)
    {
        TextUI.text = _text;
        await Task.Delay((int)(timer * 1000));
        if (autoDisappear)
            gameObject.SetActive(false);
    }
}

}