using System;


namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class LogitechRumblePad2WinProfile : UnityInputDeviceProfile {
        public LogitechRumblePad2WinProfile() {
            Name = "Logitech RumblePad 2 Controller";
            Meta = "Logitech RumblePad 2 Controller on Windows";

            SupportedPlatforms = new[] {
                "Windows"
            };

            JoystickNames = new[] {
                "Logitech Rumblepad 2 USB"
            };

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "1",
                    Target = InputTarget.Action3,
                    Source = Button0
                },
                new InputMapping {
                    Handle = "2",
                    Target = InputTarget.Action1,
                    Source = Button1
                },
                new InputMapping {
                    Handle = "3",
                    Target = InputTarget.Action2,
                    Source = Button2
                },
                new InputMapping {
                    Handle = "4",
                    Target = InputTarget.Action4,
                    Source = Button3
                },
                new InputMapping {
                    Handle = "9",
                    Target = InputTarget.Back,
                    Source = Button8
                },
                new InputMapping {
                    Handle = "10",
                    Target = InputTarget.Start,
                    Source = Button9
                },
                new InputMapping {
                    Handle = "Left Stick Button",
                    Target = InputTarget.LeftStickButton,
                    Source = Button11
                },
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button12
                },
                new InputMapping {
                    Handle = "Left Bumper",
                    Target = InputTarget.LeftBumper,
                    Source = Button4
                },
                new InputMapping {
                    Handle = "Right Bumper",
                    Target = InputTarget.RightBumper,
                    Source = Button5
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Button6
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Button7
                }
            };

            AnalogMappings = new[] {
                new InputMapping {
                    Handle = "Left Stick X",
                    Target = InputTarget.LeftStickX,
                    Source = Analog0
                },
                new InputMapping {
                    Handle = "Left Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog1,
                    Invert = true
                },
                new InputMapping {
                    Handle = "Right Stick X",
                    Target = InputTarget.RightStickX,
                    Source = Analog2
                },
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog3,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog4,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog4
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog5,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog5
                },
            };
        }
    }
}