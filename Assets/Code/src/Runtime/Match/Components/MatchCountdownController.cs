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

    void StartCooldown(MatchStartCountdownEvent evt)
    {
        // TODO: Change const 3 to a value that could changed by matchConfig
        StartCoroutine(CooldownCoroutine(3));
    }

    IEnumerator CooldownCoroutine(int timer)
    {
        // Iterate numbers
        while (timer > 0)
        {
            UpdateTextUI(timer.ToString());
            yield return new WaitForSeconds(1);
            timer--;
        }
        // TODO: Add control enabler
        // TODO: Make GO! string into localization string 
        UpdateTextUI("GO!");
        yield return new WaitForSeconds(GoDuration);
        gameObject.SetActive(false);
    }

    void UpdateTextUI(string _text)
    {
        TextUI.text = _text;
    }
}

}