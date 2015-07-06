using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

    [SerializeField]
    private Image logo;

    [SerializeField]
    private Text derivativeWarning;

    [SerializeField]
    private AnimationCurve alphaOverTime;

    [SerializeField]
    private GameObject[] disableWhileLoading;

    [SerializeField]
    private string targetSceneName;

	// Use this for initialization
	void Start () {
        StartCoroutine(DisplaySplashScreen());
	}

    IEnumerator DisplaySplashScreen() {
        foreach (GameObject target in disableWhileLoading) {
            target.SetActive(false);
        }
        float logoDisplayDuration = alphaOverTime.keys[alphaOverTime.length - 1].time;
        if (logo != null) {
            if (derivativeWarning != null)
                derivativeWarning.enabled = false;
            //Do the splash screen animation
            float t = 0;
            Color baseColor = logo.color;
            Color targetColor = baseColor;
            baseColor.a = 0f;
            while (t < logoDisplayDuration) {
                logo.color = Color.Lerp(baseColor, targetColor, alphaOverTime.Evaluate(t));
                //Wait one frame
                yield return null;
                t += Time.deltaTime;
            }
            if (derivativeWarning != null)
                derivativeWarning.enabled = true;
            logo.enabled = false;
        }
        if (derivativeWarning != null)
        {
            //Do the splash screen animation
            float t = 0;
            Color baseColor = derivativeWarning.color;
            Color targetColor = baseColor;
            baseColor.a = 0f;
            while (t < logoDisplayDuration)
            {
                derivativeWarning.color = Color.Lerp(baseColor, targetColor, alphaOverTime.Evaluate(t));
                //Wait one frame
                yield return null;
                t += Time.deltaTime;
            }
            derivativeWarning.enabled = false;
        }
        
        AsyncOperation operation = Application.LoadLevelAsync(targetSceneName);
        if (operation != null && !operation.isDone) {
            foreach (GameObject target in disableWhileLoading) {
                target.SetActive(true);
            }
            while (!operation.isDone) {
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
