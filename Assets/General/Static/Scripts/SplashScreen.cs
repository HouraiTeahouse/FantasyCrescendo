using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Vexe.Runtime.Types;

public class SplashScreen : BetterBehaviour {

    [Serialize]
    private AnimationCurve alphaOverTime;

    [Serialize]
    private GameObject[] disableWhileLoading;

    [Serialize]
    private Graphic[] splashGraphics;

    [Serialize, SelectScene]
    private string targetSceneName;

    // Use this for initialization
    private void Start() {
        StartCoroutine(DisplaySplashScreen());
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
                t += Time.deltaTime;
            }
            graphic.enabled = false;
            graphic.color = targetColor;
        }
        AsyncOperation operation = Application.LoadLevelAsync(targetSceneName);
        if (operation != null && !operation.isDone) {
            foreach (GameObject target in disableWhileLoading)
                target.SetActive(true);
            while (!operation.isDone)
                yield return null;
        }
        Destroy(gameObject);
    }

}