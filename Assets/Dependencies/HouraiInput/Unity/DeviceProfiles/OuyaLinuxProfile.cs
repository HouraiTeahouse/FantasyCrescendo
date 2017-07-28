using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class OuyaLinuxProfile : UnityInputDeviceProfile {

        public OuyaLinuxProfile() {
            Name = "OUYA Controller";
            Meta = "OUYA Controller on Linux";

            SupportedPlatforms = new[] {"Linux"};

            JoystickNames = new[] {"OUYA Game Controller"};

            LowerDeadZone = 0.3f;

            ButtonMappings = new[] {
                new InputMapping {Handle = "O", Target = InputTarget.Action1, Source = Button0},
                new InputMapping {Handle = "A", Target = InputTarget.Action2, Source = Button3},
                new InputMapping {Handle = "U", Target = InputTarget.Action3, Source = Button1},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button2},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button4},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button5},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button6},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button7
                },
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = KeyCodeButton(KeyCode.Menu)},
                new InputMapping {Handle = "TouchPad Tap", Target = InputTarget.TouchPadTap, Source = MouseButton0},
                new InputMapping {Handle = "DPad Left", Target = InputTarget.DPadLeft, Source = Button10,},
                new InputMapping {Handle = "DPad Right", Target = InputTarget.DPadRight, Source = Button11,},
                new InputMapping {Handle = "DPad Up", Target = InputTarget.DPadUp, Source = Button8,},
                new InputMapping {Handle = "DPad Down", Target = InputTarget.DPadDown, Source = Button9,},
            };

            AnalogMappings = new[] {
                new InputMapping {Handle = "Left Stick X", Target = InputTarget.LeftStickX, Source = Analog0},
                new InputMapping {
                    Handle = "Left Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog1,
                    Invert = true
                },
                new InputMapping {Handle = "Right Stick X", Target = InputTarget.RightStickX, Source = Analog3},
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog4,
                    Invert = true
                },
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Analog2},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Analog5},
                new InputMapping {
                    Handle = "TouchPad X Axis",
                    Target = InputTarget.TouchPadXAxis,
                    Source = MouseXAxis,
                    Raw = true
                },
                new InputMapping {
                    Handle = "TouchPad Y Axis",
                    Target = InputTarget.TouchPadYAxis,
                    Source = MouseYAxis,
                    Raw = true
                }
            };
        }

    }

}