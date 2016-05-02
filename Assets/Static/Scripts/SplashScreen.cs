using System.Collections;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HouraiTeahouse.SmashBrew;

public class SplashScreen : MonoBehaviour {
    [SerializeField]
    private AnimationCurve alphaOverTime;

    [SerializeField]
    private GameObject[] disableWhileLoading;

    [SerializeField]
    private Graphic[] splashGraphics;

    [SerializeField]
    private string targetSceneName;

    [SerializeField]
    private InputTarget[] _skipButtons = {InputTarget.Action1, InputTarget.Start};

    [SerializeField]
    private float _skipSpeed = 2f;

    // Use this for initialization
    private void Start() {
        StartCoroutine(DisplaySplashScreen());
    }

	private bool CheckForSkip(){
		return HInput.Devices.Any(device => device.MenuWasPressed);
	}

    private IEnumerator DisplaySplashScreen() {
        foreach (GameObject target in disableWhileLoading)
            target.SetActive(false);
        float logoDisplayDuration = alphaOverTime.keys[alphaOverTime.length - 1].time;
        foreach (Graphic graphic in splashGraphics)
            graphic.enabled = false;
        foreach (Graphic graphic in splashGraphics) {
            if (graphic == null)
                continue;
            graphic.enabled = true;
            float t = 0;
            Color baseColor = graphic.color;
            Color targetColor = baseColor;
            baseColor.a = 0f;
            while (t < logoDisplayDuration) {
                graphic.color = Color.Lerp(baseColor, targetColor, alphaOverTime.Evaluate(t));

                //Wait one frame
                yield return null;
                var skipCheck = false;
                foreach (InputDevice device in HInput.Devices)
                    skipCheck |= device.GetControls(_skipButtons).Any(control => control.State);
                t += ( skipCheck ? _skipSpeed : 1 ) * Time.deltaTime;
            }
            graphic.enabled = false;
            graphic.color = targetColor;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        if (operation != null && !operation.isDone) {
            foreach (GameObject target in disableWhileLoading)
                target.SetActive(true);
            while (!operation.isDone)
                yield return null;
        }
        Destroy(gameObject);
    }
}
