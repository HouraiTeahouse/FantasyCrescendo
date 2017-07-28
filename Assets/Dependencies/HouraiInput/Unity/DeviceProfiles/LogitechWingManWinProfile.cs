namespace HouraiTeahouse.HouraiInput {

    public class LogitechWingManWinProfile : UnityInputDeviceProfile {

        public LogitechWingManWinProfile() {
            Name = "Logitech WingMan Controller";
            Meta = "Logitech WingMan Controller on Windows";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"WingMan Cordless Gamepad",};

            ButtonMappings = new[] {
                new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button1},
                new InputMapping {Handle = "B", Target = InputTarget.Action2, Source = Button2},
                new InputMapping {Handle = "C", Target = InputTarget.Button0, Source = Button2},
                new InputMapping {Handle = "X", Target = InputTarget.Action3, Source = Button4},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button5},
                new InputMapping {Handle = "Z", Target = InputTarget.Button1, Source = Button6},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button7},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button8},
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Button10},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Button11,},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button9}
            };

            AnalogMappings = new[] {
                new InputMapping {Handle = "Left Stick X", Target = InputTarget.LeftStickX, Source = Analog0},
                new InputMapping {
                    Handle = "Left Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog1,
                    Invert = true
                },
                new InputMapping {Handle = "Right Stick X", Target = InputTarget.RightStickX, Source = Analog3},
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog4,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog5,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog5,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog6,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog6,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {Handle = "Throttle", Target = InputTarget.Analog0, Source = Analog2}
            };
        }

    }

}