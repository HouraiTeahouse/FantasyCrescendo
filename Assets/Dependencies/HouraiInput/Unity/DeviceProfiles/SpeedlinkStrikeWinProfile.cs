using System;


namespace InControl {
    // @cond nodoc
    [AutoDiscover]
    public class SpeedlinkStrikeWinProfile : UnityInputDeviceProfile {
        public SpeedlinkStrikeWinProfile() {
            Name = "Speedlink Strike Controller";
            Meta = "Speedlink Strike Controller on Windows";

            SupportedPlatforms = new[] {
                "Win"
            };

            JoystickNames = new[] {
                "SPEEDLINK STRIKE Gamepad"
            };

            ButtonMappings = new[] {
                new InputControlMapping {
                    Handle = "3",
                    Target = InputControlTarget.Action1,
                    Source = Button2
                },
                new InputControlMapping {
                    Handle = "2",
                    Target = InputControlTarget.Action2,
                    Source = Button1
                },
                new InputControlMapping {
                    Handle = "4",
                    Target = InputControlTarget.Action3,
                    Source = Button3
                },
                new InputControlMapping {
                    Handle = "1",
                    Target = InputControlTarget.Action4,
                    Source = Button0
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
                    Handle = "10",
                    Target = InputControlTarget.Start,
                    Source = Button9
                },
                new InputControlMapping {
                    Handle = "9",
                    Target = InputControlTarget.Select,
                    Source = Button8
                },
                new InputControlMapping {
                    Handle = "Left Trigger",
                    Target = InputControlTarget.LeftTrigger,
                    Source = Button6
                },
                new InputControlMapping {
                    Handle = "Right Trigger",
                    Target = InputControlTarget.RightTrigger,
                    Source = Button7
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
                    Handle = "DPad Down",
                    Target = InputControlTarget.DPadDown,
                    Source = Analog5,
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
                }
            };
        }
    }
}