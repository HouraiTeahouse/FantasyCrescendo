namespace HouraiTeahouse.HouraiInput {

    // @cond nodoc
    [AutoDiscover]
    public class PlayStationVitaPSMProfile : UnityInputDeviceProfile {

        public PlayStationVitaPSMProfile() {
            Name = "PlayStation Mobile";
            Meta = "PlayStation Mobile on Vita";

            SupportedPlatforms = new[] {"PSM UNITY FOR PSM", "PSM ON PS VITA", "PS VITA", "PSP2OS"};

            JoystickNames = new[] {"PS Vita"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "Cross", Target = InputTarget.Action1, Source = Button0},
                new InputMapping {Handle = "Circle", Target = InputTarget.Action2, Source = Button1},
                new InputMapping {Handle = "Square", Target = InputTarget.Action3, Source = Button2},
                new InputMapping {Handle = "Triangle", Target = InputTarget.Action4, Source = Button3},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button4},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button5},
                new InputMapping {Handle = "Select", Target = InputTarget.Select, Source = Button6},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button7}
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
                }
            };
        }

    }

}