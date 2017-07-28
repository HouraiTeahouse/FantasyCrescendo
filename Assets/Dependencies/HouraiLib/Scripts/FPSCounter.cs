using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace HouraiTeahouse {

    [RequireComponent(typeof(Text))]
    public sealed class FPSCounter : MonoBehaviour {

        [SerializeField]
        NetworkManager _networkManager;

        Text Counter;
        float deltaTime;
        float fps;
        string outputText;

        void Awake() {
            Counter = GetComponent<Text>();
            StartCoroutine(UpdateDisplay());
        }

        void Start() {
            if (_networkManager == null)
                _networkManager = NetworkManager.singleton;
        }

        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
        }

        IEnumerator UpdateDisplay() {
            while (true) {
                yield return new WaitForSeconds(0.5f);
                var text = "{0:0.}FPS".With(fps);
                if (_networkManager != null && _networkManager.client != null)
                    text += "/{0:0.} RTT".With(_networkManager.client.GetRTT());
                Counter.text = text;
            }
        }

    }

}
