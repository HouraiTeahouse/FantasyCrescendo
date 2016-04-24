using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class InputManager : MonoBehaviour {
        [SerializeField]
        private bool _invertYAxis = false;
        [SerializeField]
        private bool _enableXInput = false;
        [SerializeField]
        private bool _useFixedUpdate = false;
        [SerializeField]
        private bool _dontDestroyOnLoad = false;
        [SerializeField]
        private List<string> _customProfiles = new List<string>();

        private void OnEnable() {
            HInput.InvertYAxis = _invertYAxis;
            HInput.EnableXInput = _enableXInput;
            HInput.SetupInternal();

            foreach (string className in _customProfiles) {
                Type classType = Type.GetType(className);
                if (classType == null) 
                    Log.Error("Cannot find class for custom profile: {0}", className);
                else {
                    var customProfileInstance = Activator.CreateInstance(classType) as UnityInputDeviceProfile;
                    HInput.AttachDevice(new UnityInputDevice(customProfileInstance));
                }
            }

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        private void OnDisable() {
            HInput.ResetInternal();
        }

#if UNITY_ANDROID && INCONTROL_OUYA && !UNITY_EDITOR
		void Start() {
			StartCoroutine( CheckForOuyaEverywhereSupport() );
		}


		IEnumerator CheckForOuyaEverywhereSupport() {
			while (!OuyaSDK.isIAPInitComplete())
				yield return null;

			OuyaEverywhereDeviceManager.Enable();
		}
#endif

        private void Update() {
            if (!_useFixedUpdate || Mathf.Approximately(Time.timeScale, 0.0f)) 
                HInput.UpdateInternal();
        }

        private void FixedUpdate() {
            if (_useFixedUpdate)
                HInput.UpdateInternal();
        }

        private void OnApplicationFocus(bool focusState) {
            HInput.OnApplicationFocus(focusState);
        }
    }
}
