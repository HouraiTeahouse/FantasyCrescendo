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

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace HouraiTeahouse.HouraiInput {
    public class XInputDeviceManager : InputDeviceManager {
        readonly bool[] _deviceConnected = {false, false, false, false};

        public XInputDeviceManager() {
            for (var deviceIndex = 0; deviceIndex < 4; deviceIndex++)
                devices.Add(new XInputDevice(deviceIndex));

            Update(0, 0.0f);
        }


        public override void Update(ulong updateTick, float deltaTime) {
            for (var deviceIndex = 0; deviceIndex < 4; deviceIndex++) {
                var device = devices[deviceIndex] as XInputDevice;

                // Unconnected devices won't be updated otherwise, so poll here.
                if (!device.IsConnected)
                    device.Update(updateTick, deltaTime);

                if (device.IsConnected == _deviceConnected[deviceIndex])
                    continue;
                if (device.IsConnected)
                    HInput.AttachDevice(device);
                else
                    HInput.DetachDevice(device);

                _deviceConnected[deviceIndex] = device.IsConnected;
            }
        }


        public static bool CheckPlatformSupport(ICollection<string> errors) {
            if (Application.platform != RuntimePlatform.WindowsPlayer
                && Application.platform != RuntimePlatform.WindowsEditor) {
                return false;
            }

            try {
                GamePad.GetState(PlayerIndex.One);
            } catch (DllNotFoundException e) {
                if (errors != null)
                    errors.Add(e.Message + ".dll could not be found or is missing a dependency.");
                return false;
            }

            return true;
        }


        public static void Enable() {
            var errors = new List<string>();
            if (CheckPlatformSupport(errors)) {
                HInput.HideDevicesWithProfile(typeof(Xbox360WinProfile));
                HInput.HideDevicesWithProfile(typeof(XboxOneWinProfile));
                HInput.HideDevicesWithProfile(
                    typeof(LogitechF710ModeXWinProfile));
                HInput.HideDevicesWithProfile(
                    typeof(LogitechF310ModeXWinProfile));
                HInput.AddDeviceManager<XInputDeviceManager>();
            }
            else {
                foreach (string error in errors)
                    Log.Error(error);
            }
        }
    }
}

#endif
