namespace HouraiTeahouse.HouraiInput {
    public class UnityUnknownDeviceProfile : UnityInputDeviceProfile {
        public UnityUnknownDeviceProfile(string joystickName) {
            Name = "Unknown Device";
            if (joystickName != "") {
                Name += " (" + joystickName + ")";
            }

            Meta = "";
            Sensitivity = 1.0f;
            LowerDeadZone = 0.2f;

            SupportedPlatforms = null;
            JoystickNames = new[] {joystickName};

            AnalogMappings = new InputMapping[UnityInputDevice.MaxAnalogs];
            for (var i = 0; i < UnityInputDevice.MaxAnalogs; i++) {
                AnalogMappings[i] = new InputMapping {
                    Handle = "Analog " + i,
                    Source = Analog(i),
                    Target = InputTarget.Analog0 + i
                };
            }

            ButtonMappings = new InputMapping[UnityInputDevice.MaxButtons];
            for (var i = 0; i < UnityInputDevice.MaxButtons; i++) {
                ButtonMappings[i] = new InputMapping {
                    Handle = "Button " + i,
                    Source = Button(i),
                    Target = InputTarget.Button0 + i
                };
            }
        }

        public override bool IsKnown {
            get { return false; }
        }
    }
}
