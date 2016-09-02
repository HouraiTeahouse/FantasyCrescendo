using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    [RequireComponent(typeof(Text))]
    public sealed class FPSCounter : MonoBehaviour {

        Text Counter;
        float deltaTime;
        float fps;
        float msec;
        string outputText;

        void Awake() {
            Counter = GetComponent<Text>();
            StartCoroutine(UpdateDisplay());
        }

        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
        }

        IEnumerator UpdateDisplay() {
            while (true) {
                yield return new WaitForSeconds(0.5f);
                Counter.text = "{0:0.0} ms ({1:0.} fps)".With(msec, fps);
            }
        }

    }

}