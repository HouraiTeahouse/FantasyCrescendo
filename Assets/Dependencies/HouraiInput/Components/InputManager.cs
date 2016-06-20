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
                    Log.Error("Cannot find class for custom profile: {0}",
                        className);
                else {
                    var customProfileInstance =
                        Activator.CreateInstance(classType) as
                            UnityInputDeviceProfile;
                    HInput.AttachDevice(
                        new UnityInputDevice(customProfileInstance));
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

        void OnApplicationFocus(bool focusState) {
            HInput.OnApplicationFocus(focusState);
        }
    }
}