namespace HouraiTeahouse.HouraiInput {

    // @cond nodoc
    [AutoDiscover]
    public class PlayStation3WinProfile : UnityInputDeviceProfile {

        public PlayStation3WinProfile() {
            Name = "PlayStation 3 Controller";
            Meta = "PlayStation 3 Controller on Windows (via MotioninJoy Gamepad Tool)";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"MotioninJoy Virtual Game Controller"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "Cross", Target = InputTarget.Action1, Source = Button2},
                new InputMapping {Handle = "Circle", Target = InputTarget.Action2, Source = Button1},
                new InputMapping {Handle = "Square", Target = InputTarget.Action3, Source = Button3},
                new InputMapping {Handle = "Triangle", Target = InputTarget.Action4, Source = Button0},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button4},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button5},
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Button6},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Button7},
                new InputMapping {Handle = "Select", Target = InputTarget.Select, Source = Button8},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button9},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button10
                },
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button11},
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = Button12}
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
                    Source = Analog5,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog8,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog8,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog9,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog9,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {Handle = "Tilt X", Target = InputTarget.TiltX, Source = Analog3},
                new InputMapping {Handle = "Tilt Y", Target = InputTarget.TiltY, Source = Analog4}
            };
        }

    }

}