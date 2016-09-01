// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using HouraiTeahouse.SmashBrew;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

    [SerializeField]
    InputTarget[] _skipButtons = {InputTarget.Action1, InputTarget.Start};

    [SerializeField]
    float _skipSpeed = 2f;

    [SerializeField]
    AnimationCurve alphaOverTime;

    [SerializeField]
    GameObject[] disableWhileLoading;

    [SerializeField]
    Graphic[] splashGraphics;

    [SerializeField]
    string targetSceneName;

    // Use this for initialization
    void Start() { StartCoroutine(DisplaySplashScreen()); }

    bool CheckForSkip() {
        return HInput.Devices.Any(device => device.MenuWasPressed);
    }

    IEnumerator DisplaySplashScreen() {
        foreach (GameObject target in disableWhileLoading)
            target.SetActive(false);
        float logoDisplayDuration =
            alphaOverTime.keys[alphaOverTime.length - 1].time;
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
                graphic.color = Color.Lerp(baseColor,
                    targetColor,
                    alphaOverTime.Evaluate(t));

                //Wait one frame
                yield return null;
                bool skipCheck =
                    HInput.Devices.SelectMany(d => d.GetControls(_skipButtons))
                        .Any(control => control.State);
                t += (skipCheck ? _skipSpeed : 1) * Time.deltaTime;
            }
            graphic.enabled = false;
            graphic.color = targetColor;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        if (operation != null && !operation.isDone) {
            foreach (GameObject target in disableWhileLoading)
                target.SetActive(true);
            yield return operation;
        }
        Destroy(gameObject);
    }

}
