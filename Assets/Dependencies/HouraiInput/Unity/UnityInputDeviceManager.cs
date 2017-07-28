using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityInputDeviceManager : InputDeviceManager {

        const float DeviceRefreshInterval = 1.0f;

        readonly List<UnityInputDeviceProfile> _deviceProfiles = new List<UnityInputDeviceProfile>();

        float _deviceRefreshTimer;
        string _joystickHash = "";

        bool _keyboardDevicesAttached;

        public UnityInputDeviceManager() {
            AutoDiscoverDeviceProfiles();
            RefreshDevices();
        }

        static string JoystickHash {
            get {
                string[] joystickNames = Input.GetJoystickNames();
                return joystickNames.Length + ":" + string.Join(",", joystickNames);
            }
        }

        public override void Update(ulong updateTick, float deltaTime) {
            _deviceRefreshTimer += deltaTime;
            if (!string.IsNullOrEmpty(_joystickHash) && !(_deviceRefreshTimer >= DeviceRefreshInterval))
                return;
            _deviceRefreshTimer = 0.0f;

            if (_joystickHash.Equals(JoystickHash, StringComparison.OrdinalIgnoreCase))
                return;
            Log.Info("Change in Unity attached joysticks detected; refreshing device list.");
            RefreshDevices();
        }

        void RefreshDevices() {
            AttachKeyboardDevices();
            DetectAttachedJoystickDevices();
            DetectDetachedJoystickDevices();
            _joystickHash = JoystickHash;
        }

        void AttachDevice(InputDevice device) {
            devices.Add(device);
            HInput.AttachDevice(device);
        }

        void AttachKeyboardDevices() {
            foreach (UnityInputDeviceProfile deviceProfile in _deviceProfiles)
                if (!deviceProfile.IsJoystick && deviceProfile.IsSupportedOnThisPlatform)
                    AttachKeyboardDeviceWithConfig(deviceProfile);
        }

        void AttachKeyboardDeviceWithConfig(UnityInputDeviceProfile config) {
            if (_keyboardDevicesAttached)
                return;

            var keyboardDevice = new UnityInputDevice(config);
            AttachDevice(keyboardDevice);

            _keyboardDevicesAttached = true;
        }

        void DetectAttachedJoystickDevices() {
            try {
                string[] joystickNames = Input.GetJoystickNames();
                for (var i = 0; i < joystickNames.Length; i++)
                    DetectAttachedJoystickDevice(i + 1, joystickNames[i]);
            } catch (Exception e) {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);
            }
        }

        void DetectAttachedJoystickDevice(int unityJoystickId, string unityJoystickName) {
            if (unityJoystickName == "WIRED CONTROLLER" || unityJoystickName == " WIRED CONTROLLER") {
                // Ignore Steam controller for now.
                return;
            }

            if (unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) != -1) {
                // Unity thinks some webcams are joysticks. >_<
                return;
            }

            // As of Unity 4.6.3p1, empty strings on windows represent disconnected devices.
            if ((Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer) && string.IsNullOrEmpty(unityJoystickName))
                return;

            UnityInputDeviceProfile matchedDeviceProfile =
                _deviceProfiles.Find(config => config.HasJoystickName(unityJoystickName))
                    ?? _deviceProfiles.Find(config => config.HasLastResortRegex(unityJoystickName));

            UnityInputDeviceProfile deviceProfile = null;

            if (matchedDeviceProfile == null) {
                deviceProfile = new UnityUnknownDeviceProfile(unityJoystickName);
                _deviceProfiles.Add(deviceProfile);
            }
            else {
                deviceProfile = matchedDeviceProfile;
            }

            if (
                devices.OfType<UnityInputDevice>()
                    .Any(unityDevice => unityDevice.IsConfiguredWith(deviceProfile, unityJoystickId))) {
                Log.Info("Device \"{0}\" is already configured with {1}", unityJoystickName, deviceProfile.Name);
                return;
            }

            if (!deviceProfile.IsHidden) {
                var joystickDevice = new UnityInputDevice(deviceProfile, unityJoystickId);
                AttachDevice(joystickDevice);

                if (matchedDeviceProfile == null)
                    Log.Warning("Device {0} with name \"{1}\" does not match any known profiles.",
                        unityJoystickId,
                        unityJoystickName);
                else
                    Log.Info("Device {0} matched profile {1} ({2})",
                        unityJoystickId,
                        deviceProfile.GetType().Name,
                        deviceProfile.Name);
            }
            else {
                Log.Info("Device {0} matching profile {1} ({2}) is hidden and will not be attached.",
                    unityJoystickId,
                    deviceProfile.GetType().Name,
                    deviceProfile.Name);
            }
        }

        void DetectDetachedJoystickDevices() {
            string[] joystickNames = Input.GetJoystickNames();

            for (int i = devices.Count - 1; i >= 0; i--) {
                var inputDevice = devices[i] as UnityInputDevice;
                if (inputDevice == null || !inputDevice.Profile.IsJoystick)
                    continue;
                if (joystickNames.Length >= inputDevice.JoystickId
                    && inputDevice.Profile.HasJoystickOrRegexName(joystickNames[inputDevice.JoystickId - 1]))
                    continue;
                devices.Remove(inputDevice);
                HInput.DetachDevice(inputDevice);
                Log.Info("Detached device: {0}", inputDevice.Profile.Name);
            }
        }

        void AutoDiscoverDeviceProfiles() {
            foreach (string typeName in UnityInputDeviceProfileList.Profiles) {
                Type type = Type.GetType(typeName);
                if (type == null)
                    continue;
                var deviceProfile = Activator.CreateInstance(type) as UnityInputDeviceProfile;
                if (deviceProfile != null && deviceProfile.IsSupportedOnThisPlatform)
                    _deviceProfiles.Add(deviceProfile);
            }
        }

    }

}
