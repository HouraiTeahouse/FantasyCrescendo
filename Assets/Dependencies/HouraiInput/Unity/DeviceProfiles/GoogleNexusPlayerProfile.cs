using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class GoogleNexusPlayerProfile : UnityInputDeviceProfile
	{
		public GoogleNexusPlayerProfile()
		{
			Name = "Google Nexus Player Controller";
			Meta = "Google Nexus Player Controller on Android";

			SupportedPlatforms = new[] {
				"Android",
			};

			JoystickNames = new[] {
				"ASUS Gamepad",
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "A",
					Target = InputControlTarget.Action1,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "B",
					Target = InputControlTarget.Action2,
					Source = Button1
				},
				new InputControlMapping {
					Handle = "X",
					Target = InputControlTarget.Action3,
					Source = Button2
				},
				new InputControlMapping {
					Handle = "Y",
					Target = InputControlTarget.Action4,
					Source = Button3
				},
				new InputControlMapping {
					Handle = "Left Bumper",
					Target = InputControlTarget.LeftBumper,
					Source = Button4
				},
				new InputControlMapping {
					Handle = "Right Bumper",
					Target = InputControlTarget.RightBumper,
					Source = Button5
				},
				new InputControlMapping {
					Handle = "Left Stick Button",
					Target = InputControlTarget.LeftStickButton,
					Source = Button8
				},
				new InputControlMapping {
					Handle = "Right Stick Button",
					Target = InputControlTarget.RightStickButton,
					Source = Button9
				},
				new InputControlMapping {
					Handle = "Back",
					Target = InputControlTarget.Back,
					Source = KeyCodeButton( UnityEngine.KeyCode.Escape )
				},
				new InputControlMapping {
					Handle = "Start",
					Target = InputControlTarget.Start,
					Source = Button10
				}
			};

			AnalogMappings = new[] {
				new InputControlMapping {
					Handle = "Left Stick X",
					Target = InputControlTarget.LeftStickX,
					Source = Analog0
				},
				new InputControlMapping {
					Handle = "Left Stick Y",
					Target = InputControlTarget.LeftStickY,
					Source = Analog1,
					Invert = true
				},
				new InputControlMapping {
					Handle = "Right Stick X",
					Target = InputControlTarget.RightStickX,
					Source = Analog2
				},
				new InputControlMapping {
					Handle = "Right Stick Y",
					Target = InputControlTarget.RightStickY,
					Source = Analog3,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Left",
					Target = InputControlTarget.DPadLeft,
					Source = Analog4,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Right",
					Target = InputControlTarget.DPadRight,
					Source = Analog4,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Up",
					Target = InputControlTarget.DPadUp,
					Source = Analog5,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Down",
					Target = InputControlTarget.DPadDown,
					Source = Analog5,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive,
				},
				new InputControlMapping {
					Handle = "Left Trigger",
					Target = InputControlTarget.LeftTrigger,
					Source = Analog12,
				},
				new InputControlMapping {
					Handle = "Right Trigger",
					Target = InputControlTarget.RightTrigger,
					Source = Analog11,
				}
			};
		}
	}
}

