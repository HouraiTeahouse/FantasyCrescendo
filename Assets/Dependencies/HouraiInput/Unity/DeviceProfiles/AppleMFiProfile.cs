using System;


namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class AppleMFiProfile : UnityInputDeviceProfile {
        public AppleMFiProfile() {
            Name = "Apple MFi Controller";
            Meta = "Apple MFi Controller on iOS";

            SupportedPlatforms = new[] {
                "iPhone"
            };

            LastResortRegex = ""; // Match anything.

            LowerDeadZone = 0.05f;
            UpperDeadZone = 0.95f;

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "A",
                    Target = InputTarget.Action1,
                    Source = Button14
                },
                new InputMapping {
                    Handle = "B",
                    Target = InputTarget.Action2,
                    Source = Button13
                },
                new InputMapping {
                    Handle = "X",
                    Target = InputTarget.Action3,
                    Source = Button15
                },
                new InputMapping {
                    Handle = "Y",
                    Target = InputTarget.Action4,
                    Source = Button12
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Button4
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Button6
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Button7
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Button5
                },
                new InputMapping {
                    Handle = "Left Bumper",
                    Target = InputTarget.LeftBumper,
                    Source = Button8
                },
                new InputMapping {
                    Handle = "Right Bumper",
                    Target = InputTarget.RightBumper,
                    Source = Button9
                },
                new InputMapping {
                    Handle = "Pause",
                    Target = InputTarget.Pause,
                    Source = Button0
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Button10
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
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
                }
            };
        }
    }
}