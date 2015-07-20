using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(Text))]
    public sealed class FPSCounter : MonoBehaviour
    {

        private Text Counter;

        float deltaTime = 0.0f;
        private float msec;
        private float fps;
        private string outputText;

        void Awake()
        {
            Counter = GetComponent<Text>();
            StartCoroutine(UpdateDisplay());
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
        }

        IEnumerator UpdateDisplay()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                Counter.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            }
        }
    }

}

