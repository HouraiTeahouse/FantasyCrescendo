using System;


namespace HouraiTeahouse.HouraiInput {
    // Tested with ADT-1
    // Profile by Artūras 'arturaz' Šlajus <arturas@tinylabproductions.com>
    //
    // @cond nodoc
    [AutoDiscover]
    public class AndroidTVRemoteProfile : UnityInputDeviceProfile {
        public AndroidTVRemoteProfile() {
            Name = "Android TV Remote";
            Meta = "Android TV Remotet on Android TV";

            SupportedPlatforms = new[] {
                "Android"
            };

            JoystickNames = new[] {
                "touch-input",
                "navigation-input"
            };

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "A",
                    Target = InputTarget.Action1,
                    Source = Button0
                }
            };

            AnalogMappings = new[] {
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
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog5,
                    SourceRange = InputMapping.Range.Negative,
                    TargetRange = InputMapping.Range.Negative
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog5,
                    SourceRange = InputMapping.Range.Positive,
                    TargetRange = InputMapping.Range.Positive,
                    Invert = true
                },
            };
        }
    }
}