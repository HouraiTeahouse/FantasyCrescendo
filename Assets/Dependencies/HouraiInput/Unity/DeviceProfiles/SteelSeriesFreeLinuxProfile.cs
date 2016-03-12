using System;


namespace InControl {
    // @cond nodoc
    [AutoDiscover]
    public class SteelSeriesFreeLinuxProfile : UnityInputDeviceProfile {
        public SteelSeriesFreeLinuxProfile() {
            Name = "SteelSeries Free";
            Meta = "SteelSeries Free on Linux";

            SupportedPlatforms = new[] {
                "Linux",
            };

            JoystickNames = new[] {
                "Zeemote: SteelSeries FREE"
            };

            ButtonMappings = new[] {
                new InputControlMapping {
                    Handle = "4",
                    Target = InputControlTarget.Action1,
                    Source = Button0
                },
                new InputControlMapping {
                    Handle = "3",
                    Target = InputControlTarget.Action2,
                    Source = Button1
                },
                new InputControlMapping {
                    Handle = "1",
                    Target = InputControlTarget.Action3,
                    Source = Button3
                },
                new InputControlMapping {
                    Handle = "2",
                    Target = InputControlTarget.Action4,
                    Source = Button4
                },
                new InputControlMapping {
                    Handle = "Left Bumper",
                    Target = InputControlTarget.LeftBumper,
                    Source = Button6
                },
                new InputControlMapping {
                    Handle = "Right Bumper",
                    Target = InputControlTarget.RightBumper,
                    Source = Button7
                },
                new InputControlMapping {
                    Handle = "Back",
                    Target = InputControlTarget.Select,
                    Source = Button12
                },
                new InputControlMapping {
                    Handle = "Start",
                    Target = InputControlTarget.Start,
                    Source = Button11
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
                    Handle = "DPad Left",
                    Target = InputControlTarget.DPadLeft,
                    Source = Analog4,
                    SourceRange = InputControlMapping.Range.Negative,
                    TargetRange = InputControlMapping.Range.Negative,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "DPad Right",
                    Target = InputControlTarget.DPadRight,
                    Source = Analog4,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "DPad Up",
                    Target = InputControlTarget.DPadUp,
                    Source = Analog5,
                    SourceRange = InputControlMapping.Range.Negative,
                    TargetRange = InputControlMapping.Range.Negative,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "DPad Down",
                    Target = InputControlTarget.DPadDown,
                    Source = Analog5,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive,
                    Invert = false
                }
            };
        }
    }
}