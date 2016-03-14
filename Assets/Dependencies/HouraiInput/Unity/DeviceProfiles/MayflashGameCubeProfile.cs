using System;

namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class MayflashGameCubeWinProfile : UnityInputDeviceProfile {
        public MayflashGameCubeWinProfile() {
            // Gamecube Controller Adapter for PC USB
            Name = "GameCube Controller";
            Meta = "GameCube Controller on Windows";

            SupportedPlatforms = new[] {
                "Windows"
            };

            JoystickNames = new[] {
                "MAYFLASH GameCube Controller Adapter"
            };

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "A",
                    Target = InputTarget.Action1,
                    Source = Button1
                },
                new InputMapping {
                    Handle = "B",
                    Target = InputTarget.Action2,
                    Source = Button0
                },
                new InputMapping {
                    Handle = "X",
                    Target = InputTarget.Action2,
                    Source = Button2
                },
                new InputMapping {
                    Handle = "Y",
                    Target = InputTarget.Action4,
                    Source = Button3
                },
                new InputMapping {
                    Handle = "Start",
                    Target = InputTarget.Start,
                    Source = Button9
                },
                new InputMapping {
                    Handle = "Z",
                    Target = InputTarget.RightBumper,
                    Source = Button7
                },
                new InputMapping {
                    Handle = "L",
                    Target = InputTarget.LeftTrigger,
                    Source = Button4
                },
                new InputMapping {
                    Handle = "R",
                    Target = InputTarget.RightTrigger,
                    Source = Button5
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Button12
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Button14
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Button15
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Button13
                }
            };

            AnalogMappings = new[] {
                new InputMapping {
                    Handle = "Control Stick X",
                    Target = InputTarget.LeftStickX,
                    Source = Analog0
                },
                new InputMapping {
                    Handle = "Control Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog1
                },
                new InputMapping {
                    Handle = "C Stick X",
                    Target = InputTarget.RightStickX,
                    Source = Analog5
                },
                new InputMapping {
                    Handle = "C Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog2
                }
            };
        }
    }

    // @endcond
}