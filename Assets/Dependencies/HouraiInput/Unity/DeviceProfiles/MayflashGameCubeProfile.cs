using System;

namespace InControl {
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
                new InputControlMapping {
                    Handle = "A",
                    Target = InputControlTarget.Action1,
                    Source = Button1
                },
                new InputControlMapping {
                    Handle = "B",
                    Target = InputControlTarget.Action2,
                    Source = Button0
                },
                new InputControlMapping {
                    Handle = "X",
                    Target = InputControlTarget.Action2,
                    Source = Button2
                },
                new InputControlMapping {
                    Handle = "Y",
                    Target = InputControlTarget.Action4,
                    Source = Button3
                },
                new InputControlMapping {
                    Handle = "Start",
                    Target = InputControlTarget.Start,
                    Source = Button9
                },
                new InputControlMapping {
                    Handle = "Z",
                    Target = InputControlTarget.RightBumper,
                    Source = Button7
                },
                new InputControlMapping {
                    Handle = "L",
                    Target = InputControlTarget.LeftTrigger,
                    Source = Button4
                },
                new InputControlMapping {
                    Handle = "R",
                    Target = InputControlTarget.RightTrigger,
                    Source = Button5
                },
                new InputControlMapping {
                    Handle = "DPad Up",
                    Target = InputControlTarget.DPadUp,
                    Source = Button12
                },
                new InputControlMapping {
                    Handle = "DPad Down",
                    Target = InputControlTarget.DPadDown,
                    Source = Button14
                },
                new InputControlMapping {
                    Handle = "DPad Left",
                    Target = InputControlTarget.DPadLeft,
                    Source = Button15
                },
                new InputControlMapping {
                    Handle = "DPad Right",
                    Target = InputControlTarget.DPadRight,
                    Source = Button13
                }
            };

            AnalogMappings = new [] {
                new InputControlMapping {
                    Handle = "Control Stick X",
                    Target = InputControlTarget.LeftStickX,
                    Source = Analog0
                },
                new InputControlMapping {
                    Handle = "Control Stick Y",
                    Target = InputControlTarget.LeftStickY,
                    Source = Analog1
                },
                new InputControlMapping {
                    Handle = "C Stick X",
                    Target = InputControlTarget.RightStickX,
                    Source = Analog5
                },
                new InputControlMapping {
                    Handle = "C Stick Y",
                    Target = InputControlTarget.RightStickY,
                    Source = Analog2
                }
            };
        }
    }
    // @endcond
}