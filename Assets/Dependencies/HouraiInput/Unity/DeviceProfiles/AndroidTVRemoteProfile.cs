namespace HouraiTeahouse.HouraiInput {

    // Tested with ADT-1
    // Profile by Artūras 'arturaz' Šlajus <arturas@tinylabproductions.com>
    //

    public class AndroidTVRemoteProfile : UnityInputDeviceProfile {

        public AndroidTVRemoteProfile() {
            Name = "Android TV Remote";
            Meta = "Android TV Remotet on Android TV";

            SupportedPlatforms = new[] {"Android"};

            JoystickNames = new[] {"touch-input", "navigation-input"};

            ButtonMappings = new[] {new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button0}};

            AnalogMappings = new[] {
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog4,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog4,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog5,
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog5,
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive,
                    Invert = true
                },
            };
        }

    }

}
