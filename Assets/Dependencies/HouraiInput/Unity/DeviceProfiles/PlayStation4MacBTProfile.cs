namespace HouraiTeahouse.HouraiInput {

    // @cond nodoc
    [AutoDiscover]
    public class PlayStation4MacBTProfile : UnityInputDeviceProfile {

        public PlayStation4MacBTProfile() {
            Name = "PlayStation 4 Controller";
            Meta = "PlayStation 4 Controller on Mac";

            SupportedPlatforms = new[] {"OS X"};

            JoystickNames = new[] {
                "Unknown Wireless Controller" // Sigh.
            };

            ButtonMappings = new[] {
                new InputMapping {Handle = "Cross", Target = InputTarget.Action1, Source = Button1},
                new InputMapping {Handle = "Circle", Target = InputTarget.Action2, Source = Button2},
                new InputMapping {Handle = "Square", Target = InputTarget.Action3, Source = Button0},
                new InputMapping {Handle = "Triangle", Target = InputTarget.Action4, Source = Button3},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button4},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button5},
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = Button12},
                new InputMapping {Handle = "Options", Target = InputTarget.Select, Source = Button9},
                new InputMapping {Handle = "Share", Target = InputTarget.Share, Source = Button8},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button10},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button11
                },
                new InputMapping {Handle = "TouchPad Button", Target = InputTarget.TouchPadTap, Source = Button13}
            };

            AnalogMappings = new[] {
                new InputMapping {Handle = "Left Stick X", Target = InputTarget.LeftStickX, Source = Analog0},
                new InputMapping {
                    Handle = "Left Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog1,
                    Invert = true
                },
                new InputMapping {Handle = "Right Stick X", Target = InputTarget.RightStickX, Source = Analog2},
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog3,
                    Invert = true
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Analog4,
                    TargetRange = InputMapping.Positive,
                    IgnoreInitialZeroValue = true
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Analog5,
                    TargetRange = InputMapping.Positive,
                    IgnoreInitialZeroValue = true
                },

                // OS X 10.9
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog10,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog10,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog11,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog11,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },

                // OS X 10.10
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog6,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog6,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog7,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog7,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
            };
        }

    }

}