using System;


namespace HouraiTeahouse.HouraiInput {
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
                new InputMapping {
                    Handle = "3",
                    Target = InputTarget.Action1,
                    Source = Button2
                },
                new InputMapping {
                    Handle = "2",
                    Target = InputTarget.Action2,
                    Source = Button1
                },
                new InputMapping {
                    Handle = "4",
                    Target = InputTarget.Action3,
                    Source = Button3
                },
                new InputMapping {
                    Handle = "1",
                    Target = InputTarget.Action4,
                    Source = Button0
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
                    Handle = "10",
                    Target = InputTarget.Start,
                    Source = Button9
                },
                new InputMapping {
                    Handle = "9",
                    Target = InputTarget.Select,
                    Source = Button8
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
                },
                new InputMapping {
                    Handle = "Left Stick Button",
                    Target = InputTarget.LeftStickButton,
                    Source = Button10
                },
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button11
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
                    SourceRange = InputMapping.Range.Negative,
                    TargetRange = InputMapping.Range.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog4,
                    SourceRange = InputMapping.Range.Positive,
                    TargetRange = InputMapping.Range.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog5,
                    SourceRange = InputMapping.Range.Positive,
                    TargetRange = InputMapping.Range.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog5,
                    SourceRange = InputMapping.Range.Negative,
                    TargetRange = InputMapping.Range.Negative,
                    Invert = true
                }
            };
        }
    }
}