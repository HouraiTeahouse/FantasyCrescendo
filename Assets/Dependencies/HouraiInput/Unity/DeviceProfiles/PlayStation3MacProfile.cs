using System;


namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class PlayStation3MacProfile : UnityInputDeviceProfile {
        public PlayStation3MacProfile() {
            Name = "PlayStation 3 Controller";
            Meta = "PlayStation 3 Controller on Mac";

            SupportedPlatforms = new[] {
                "OS X"
            };

            JoystickNames = new[] {
                "Sony PLAYSTATION(R)3 Controller",
                "SHENGHIC 2009/0708ZXW-V1Inc. PLAYSTATION(R)3Conteroller" // Works in editor, not in player?
            };

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "Cross",
                    Target = InputTarget.Action1,
                    Source = Button14
                },
                new InputMapping {
                    Handle = "Circle",
                    Target = InputTarget.Action2,
                    Source = Button13
                },
                new InputMapping {
                    Handle = "Square",
                    Target = InputTarget.Action3,
                    Source = Button15
                },
                new InputMapping {
                    Handle = "Triangle",
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
                    Source = Button10
                },
                new InputMapping {
                    Handle = "Right Bumper",
                    Target = InputTarget.RightBumper,
                    Source = Button11
                },
                new InputMapping {
                    Handle = "Start",
                    Target = InputTarget.Start,
                    Source = Button3
                },
                new InputMapping {
                    Handle = "Select",
                    Target = InputTarget.Select,
                    Source = Button0
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Button8
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Button9
                },
                new InputMapping {
                    Handle = "Left Stick Button",
                    Target = InputTarget.LeftStickButton,
                    Source = Button1
                },
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button2
                },
                new InputMapping {
                    Handle = "System",
                    Target = InputTarget.System,
                    Source = Button16
                },
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