using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

    [SerializeField]
    private Image logo;

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
        if (logo != null) {
            //Do the splash screen animation
            float t = 0;
            float logoDisplayDuration = alphaOverTime.keys[alphaOverTime.length - 1].time;
            Color baseColor = logo.color;
            Color targetColor = baseColor;
            baseColor.a = 0f;
            while (t < logoDisplayDuration) {
                logo.color = Color.Lerp(baseColor, targetColor, alphaOverTime.Evaluate(t));
                //Wait one frame
                yield return null;
                t += Time.deltaTime;
            }
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
