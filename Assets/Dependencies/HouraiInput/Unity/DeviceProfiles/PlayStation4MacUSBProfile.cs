using System;


namespace InControl {
    // @cond nodoc
    [AutoDiscover]
    public class PlayStation4MacUSBProfile : UnityInputDeviceProfile {
        public PlayStation4MacUSBProfile() {
            Name = "PlayStation 4 Controller";
            Meta = "PlayStation 4 Controller on Mac";

            SupportedPlatforms = new[] {
                "OS X"
            };

            JoystickNames = new[] {
                "Sony Computer Entertainment Wireless Controller"
            };

            ButtonMappings = new[] {
                new InputControlMapping {
                    Handle = "Cross",
                    Target = InputControlTarget.Action1,
                    Source = Button1
                },
                new InputControlMapping {
                    Handle = "Circle",
                    Target = InputControlTarget.Action2,
                    Source = Button2
                },
                new InputControlMapping {
                    Handle = "Square",
                    Target = InputControlTarget.Action3,
                    Source = Button0
                },
                new InputControlMapping {
                    Handle = "Triangle",
                    Target = InputControlTarget.Action4,
                    Source = Button3
                },
                new InputControlMapping {
                    Handle = "Left Bumper",
                    Target = InputControlTarget.LeftBumper,
                    Source = Button4
                },
                new InputControlMapping {
                    Handle = "Right Bumper",
                    Target = InputControlTarget.RightBumper,
                    Source = Button5
                },
                new InputControlMapping {
                    Handle = "Share",
                    Target = InputControlTarget.Share,
                    Source = Button8
                },
                new InputControlMapping {
                    Handle = "Options",
                    Target = InputControlTarget.Select,
                    Source = Button9
                },
                new InputControlMapping {
                    Handle = "Left Stick Button",
                    Target = InputControlTarget.LeftStickButton,
                    Source = Button10
                },
                new InputControlMapping {
                    Handle = "Right Stick Button",
                    Target = InputControlTarget.RightStickButton,
                    Source = Button11
                },
                new InputControlMapping {
                    Handle = "System",
                    Target = InputControlTarget.System,
                    Source = Button12
                },
                new InputControlMapping {
                    Handle = "TouchPad Button",
                    Target = InputControlTarget.TouchPadTap,
                    Source = Button13
                }
            };

            AnalogMappings = new[] {
                new InputControlMapping {
                    Handle = "Left Stick X",
                    Target = InputControlTarget.LeftStickX,
                    Source = Analog0
                },
                new InputControlMapping {
                    Handle = "Left Stick Y",
                    Target = InputControlTarget.LeftStickY,
                    Source = Analog1,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "Right Stick X",
                    Target = InputControlTarget.RightStickX,
                    Source = Analog2
                },
                new InputControlMapping {
                    Handle = "Right Stick Y",
                    Target = InputControlTarget.RightStickY,
                    Source = Analog3,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "Left Trigger",
                    Target = InputControlTarget.LeftTrigger,
                    Source = Analog4,
                    TargetRange = InputControlMapping.Range.Positive,
                    IgnoreInitialZeroValue = true
                },
                new InputControlMapping {
                    Handle = "Right Trigger",
                    Target = InputControlTarget.RightTrigger,
                    Source = Analog5,
                    TargetRange = InputControlMapping.Range.Positive,
                    IgnoreInitialZeroValue = true
                },
                new InputControlMapping {
                    Handle = "DPad Left",
                    Target = InputControlTarget.DPadLeft,
                    Source = Analog6,
                    SourceRange = InputControlMapping.Range.Negative,
                    TargetRange = InputControlMapping.Range.Negative,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "DPad Right",
                    Target = InputControlTarget.DPadRight,
                    Source = Analog6,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "DPad Down",
                    Target = InputControlTarget.DPadDown,
                    Source = Analog7,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "DPad Up",
                    Target = InputControlTarget.DPadUp,
                    Source = Analog7,
                    SourceRange = InputControlMapping.Range.Negative,
                    TargetRange = InputControlMapping.Range.Negative,
                    Invert = true
                },
            };
        }
    }
}