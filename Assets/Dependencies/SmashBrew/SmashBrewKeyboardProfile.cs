using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class SmashBrewKeyboardProfile : UnityInputDeviceProfile {
        public SmashBrewKeyboardProfile() {
            Name = "Smash Brew Single Player Keybard Profile";
            Meta = "Smash Brew Single Player Keybard Profile";

            SupportedPlatforms = new[] {
                "Windows",
                "OS X", 
                "Linux"
            };

            ButtonMappings = new[] {
                new InputMapping {
                    Handle = "Attack",
                    Target = InputTarget.Action1,
                    Source = KeyCodeButton(KeyCode.Z) 
                },
                new InputMapping {
                    Handle = "Special",
                    Target = InputTarget.Action2,
                    Source = KeyCodeButton(KeyCode.X) 
                },
                new InputMapping {
                    Handle = "Jump",
                    Target = InputTarget.Action3,
                    Source = KeyCodeButton(KeyCode.LeftShift) 
                },
                new InputMapping {
                    Handle = "Start",
                    Target = InputTarget.Start,
                    Source = KeyCodeButton(KeyCode.Return) 
                },
            };

            AnalogMappings = new[] {
                new InputMapping {
                    Handle = "Left Horizontal",
                    Target = InputTarget.LeftStickX,
                    Source = KeyCodeAxis(KeyCode.LeftArrow, KeyCode.RightArrow) 
                },
                new InputMapping {
                    Handle = "Left Vertical",
                    Target = InputTarget.LeftStickY,
                    Source = KeyCodeAxis(KeyCode.DownArrow, KeyCode.UpArrow)
                },
                new InputMapping {
                    Handle = "Right Horizontal",
                    Target = InputTarget.RightStickX,
                    Source = KeyCodeAxis(KeyCode.S, KeyCode.W) 
                },
                new InputMapping {
                    Handle = "Right Vertical",
                    Target = InputTarget.RightStickY,
                    Source = KeyCodeAxis(KeyCode.A, KeyCode.D),
                }
            };
        }
    }
}
