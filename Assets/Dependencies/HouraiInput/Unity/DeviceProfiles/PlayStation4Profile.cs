using System;


namespace InControl {
    // @cond nodoc
    [AutoDiscover]
    public class PlayStation4Profile : UnityInputDeviceProfile {
        public PlayStation4Profile() {
            Name = "PlayStation 4 Controller";
            Meta = "PlayStation 4 Controller on PlayStation 4";

            SupportedPlatforms = new[] {
                "PS4"
            };

            JoystickRegex = new[] {
                "controller"
            };

            ButtonMappings = new[] {
                new InputControlMapping {
                    Handle = "Cross",
                    Target = InputControlTarget.Action1,
                    Source = Button0
                },
                new InputControlMapping {
                    Handle = "Circle",
                    Target = InputControlTarget.Action2,
                    Source = Button1
                },
                new InputControlMapping {
                    Handle = "Square",
                    Target = InputControlTarget.Action3,
                    Source = Button2
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
                    Handle = "Options",
                    Target = InputControlTarget.Select,
                    Source = Button7
                },
                new InputControlMapping {
                    Handle = "Left Stick Button",
                    Target = InputControlTarget.LeftStickButton,
                    Source = Button8
                },
                new InputControlMapping {
                    Handle = "Right Stick Button",
                    Target = InputControlTarget.RightStickButton,
                    Source = Button9
                }
            };

            AnalogMappings = new[] {
                new InputControlMapping {
                    Handle = "Left Trigger",
                    Target = InputControlTarget.LeftTrigger,
                    Source = Analog7
                },
                new InputControlMapping {
                    Handle = "Right Trigger",
                    Target = InputControlTarget.RightTrigger,
                    Source = Analog2,
                    Invert = true
                },
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
                    Source = Analog3
                },
                new InputControlMapping {
                    Handle = "Right Stick Y",
                    Target = InputControlTarget.RightStickY,
                    Source = Analog4,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "DPad Left",
                    Target = InputControlTarget.DPadLeft,
                    Source = Analog5,
                    SourceRange = InputControlMapping.Range.Negative,
                    TargetRange = InputControlMapping.Range.Negative,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "DPad Right",
                    Target = InputControlTarget.DPadRight,
                    Source = Analog5,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "DPad Up",
                    Target = InputControlTarget.DPadUp,
                    Source = Analog6,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "DPad Down",
                    Target = InputControlTarget.DPadDown,
                    Source = Analog6,
                    SourceRange = InputControlMapping.Range.Negative,
                    TargetRange = InputControlMapping.Range.Negative,
                    Invert = true
                }
            };
        }
    }
}