namespace HouraiTeahouse.HouraiInput {

    public class XboxOneMacProfile : UnityInputDeviceProfile {

        public XboxOneMacProfile() {
            Name = "XBox One Controller";
            Meta = "XBox One Controller on OSX";

            SupportedPlatforms = new[] {"OS X"};

            JoystickNames = new[] {"Microsoft Xbox One Wired Controller"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button16},
                new InputMapping {Handle = "B", Target = InputTarget.Action2, Source = Button17},
                new InputMapping {Handle = "X", Target = InputTarget.Action3, Source = Button18},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button19},
                new InputMapping {Handle = "DPad Left", Target = InputTarget.DPadLeft, Source = Button7},
                new InputMapping {Handle = "DPad Right", Target = InputTarget.DPadRight, Source = Button8},
                new InputMapping {Handle = "DPad Up", Target = InputTarget.DPadUp, Source = Button5},
                new InputMapping {Handle = "DPad Down", Target = InputTarget.DPadDown, Source = Button6,},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button13},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button14},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button11},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button12
                },
                new InputMapping {Handle = "View", Target = InputTarget.View, Source = Button10},
                new InputMapping {Handle = "Menu", Target = InputTarget.Menu, Source = Button9},
                new InputMapping {Handle = "Guide", Target = InputTarget.System, Source = Button15}
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
                    SourceRange = InputMapping.Complete,
                    TargetRange = InputMapping.Positive,
                    IgnoreInitialZeroValue = true
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Analog5,
                    SourceRange = InputMapping.Complete,
                    TargetRange = InputMapping.Positive,
                    IgnoreInitialZeroValue = true
                }
            };
        }

    }

}