using System;
using System.Collections.Generic;
using UnityEngine;


namespace HouraiTeahouse.HouraiInput {
    public class InputManager : MonoBehaviour {
        public bool logDebugInfo = false;
        public bool invertYAxis = false;
        public bool enableXInput = false;
        public bool useFixedUpdate = false;
        public bool dontDestroyOnLoad = false;
        public List<string> customProfiles = new List<string>();


        void OnEnable() {
            if (logDebugInfo) {
                Debug.Log("InControl (version " + HInput.Version + ")");
                Logger.OnLogMessage += HandleOnLogMessage;
            }

            HInput.InvertYAxis = invertYAxis;
            HInput.EnableXInput = enableXInput;
            HInput.SetupInternal();

            foreach (string className in customProfiles) {
                Type classType = Type.GetType(className);
                if (classType == null) {
                    Debug.LogError("Cannot find class for custom profile: " + className);
                }
                else {
                    var customProfileInstance = Activator.CreateInstance(classType) as UnityInputDeviceProfile;
                    HInput.AttachDevice(new UnityInputDevice(customProfileInstance));
                }
            }

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(this);
            }
        }


        void OnDisable() {
            HInput.ResetInternal();
        }


#if UNITY_ANDROID && INCONTROL_OUYA && !UNITY_EDITOR
		void Start()
		{
			StartCoroutine( CheckForOuyaEverywhereSupport() );
		}


		IEnumerator CheckForOuyaEverywhereSupport()
		{
			while (!OuyaSDK.isIAPInitComplete())
			{
				yield return null;
			}

			OuyaEverywhereDeviceManager.Enable();
		}
		#endif


        void Update() {
            if (!useFixedUpdate || Mathf.Approximately(Time.timeScale, 0.0f)) {
                HInput.UpdateInternal();
            }
        }


        void FixedUpdate() {
            if (useFixedUpdate) {
                HInput.UpdateInternal();
            }
        }


        void OnApplicationFocus(bool focusState) {
            HInput.OnApplicationFocus(focusState);
        }


        void OnApplicationPause(bool pauseState) {
            HInput.OnApplicationPause(pauseState);
        }


        void OnApplicationQuit() {
            HInput.OnApplicationQuit();
        }


        void HandleOnLogMessage(LogMessage logMessage) {
            switch (logMessage.type) {
                case LogMessageType.Info:
                    Debug.Log(logMessage.text);
                    break;
                case LogMessageType.Warning:
                    Debug.LogWarning(logMessage.text);
                    break;
                case LogMessageType.Error:
                    Debug.LogError(logMessage.text);
                    break;
            }
        }
    }
}
