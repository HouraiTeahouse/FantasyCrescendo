using System;


namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class Xbox360LinuxProfile : UnityInputDeviceProfile {
        public Xbox360LinuxProfile() {
            Name = "XBox 360 Controller";
            Meta = "XBox 360 Controller on Linux";

            SupportedPlatforms = new[] {
                "Linux"
            };

            JoystickNames = new[] {
                "Microsoft X-Box 360 pad",
                "Generic X-Box pad"
            };

            LastResortRegex = "360";

            MaxUnityVersion = new VersionInfo(4, 9);

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "A",
                    Target = InputTarget.Action1,
                    Source = Button0
                },
                new InputMapping {
                    Handle = "B",
                    Target = InputTarget.Action2,
                    Source = Button1
                },
                new InputMapping {
                    Handle = "X",
                    Target = InputTarget.Action3,
                    Source = Button2
                },
                new InputMapping {
                    Handle = "Y",
                    Target = InputTarget.Action4,
                    Source = Button3
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
                    Handle = "Left Stick Button",
                    Target = InputTarget.LeftStickButton,
                    Source = Button9
                },
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button10
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Button11,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Button12,
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Button13,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Button14,
                },
                new InputMapping {
                    Handle = "Back",
                    Target = InputTarget.Back,
                    Source = Button6
                },
                new InputMapping {
                    Handle = "Start",
                    Target = InputTarget.Start,
                    Source = Button7
                },
                new InputMapping {
                    Handle = "Menu",
                    Target = InputTarget.Menu,
                    Source = Button8
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
                    Source = Analog3
                },
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog4,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog6,
                    SourceRange = InputMapping.Range.Negative,
                    TargetRange = InputMapping.Range.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog6,
                    SourceRange = InputMapping.Range.Positive,
                    TargetRange = InputMapping.Range.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog7,
                    SourceRange = InputMapping.Range.Negative,
                    TargetRange = InputMapping.Range.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog7,
                    SourceRange = InputMapping.Range.Positive,
                    TargetRange = InputMapping.Range.Positive
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Analog2
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Analog5
                }
            };
        }
    }
}