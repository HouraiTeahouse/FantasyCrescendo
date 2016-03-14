using System;


namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class ExecutionerXProfile : UnityInputDeviceProfile {
        public ExecutionerXProfile() {
            Name = "Executioner X Controller";
            Meta = "Executioner X Controller";

            SupportedPlatforms = new[] {
                "Windows",
                "OS X"
            };

            JoystickNames = new[] {
                "Zeroplus PS Vibration Feedback Converter",
                "Zeroplus PS Vibration Feedback Converter "
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
                    Source = Button6
                },
                new InputMapping {
                    Handle = "Right Bumper",
                    Target = InputTarget.RightBumper,
                    Source = Button7
                },
                new InputMapping {
                    Handle = "Start",
                    Target = InputTarget.Start,
                    Source = Button11
                },
                new InputMapping {
                    Handle = "Options",
                    Target = InputTarget.Select,
                    Source = Button8
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Button4
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
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
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog7,
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
                }
            };
        }
    }
}