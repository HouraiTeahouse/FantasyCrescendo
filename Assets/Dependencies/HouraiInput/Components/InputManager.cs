using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class InputManager : MonoBehaviour {

        [SerializeField]
        bool _invertYAxis = false;

        [SerializeField]
        bool _enableXInput = false;

        [SerializeField]
        bool _useFixedUpdate = false;

        [SerializeField]
        bool _dontDestroyOnLoad = false;

        [SerializeField]
        List<string> _customProfiles = new List<string>();

        void OnEnable() {
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

        void OnDisable() { HInput.ResetInternal(); }

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

        void Update() {
            if (!_useFixedUpdate || Mathf.Approximately(Time.timeScale, 0.0f))
                HInput.UpdateInternal();
        }

        void FixedUpdate() {
            if (_useFixedUpdate)
                HInput.UpdateInternal();
        }

        void OnApplicationFocus(bool focusState) { HInput.OnApplicationFocus(focusState); }

    }

}