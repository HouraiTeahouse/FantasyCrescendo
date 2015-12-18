using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class PlayStation3WinProfile : UnityInputDeviceProfile
	{
		public PlayStation3WinProfile()
		{
			Name = "PlayStation 3 Controller";
			Meta = "PlayStation 3 Controller on Windows (via MotioninJoy Gamepad Tool)";

			SupportedPlatforms = new[] {
				"Windows"
			};

			JoystickNames = new[] {
				"MotioninJoy Virtual Game Controller"
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "Cross",
					Target = InputControlTarget.Action1,
					Source = Button2
				},
				new InputControlMapping {
					Handle = "Circle",
					Target = InputControlTarget.Action2,
					Source = Button1
				},
				new InputControlMapping {
					Handle = "Square",
					Target = InputControlTarget.Action3,
					Source = Button3
				},
				new InputControlMapping {
					Handle = "Triangle",
					Target = InputControlTarget.Action4,
					Source = Button0
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
					Handle = "Left Trigger",
					Target = InputControlTarget.LeftTrigger,
					Source = Button6
				},
				new InputControlMapping {
					Handle = "Right Trigger",
					Target = InputControlTarget.RightTrigger,
					Source = Button7
				},
				new InputControlMapping {
					Handle = "Select",
					Target = InputControlTarget.Select,
					Source = Button8
				},
				new InputControlMapping {
					Handle = "Left Stick Button",
					Target = InputControlTarget.LeftStickButton,
					Source = Button9
				},
				new InputControlMapping {
					Handle = "Right Stick Button",
					Target = InputControlTarget.RightStickButton,
					Source = Button10
				},
				new InputControlMapping {
					Handle = "Start",
					Target = InputControlTarget.Start,
					Source = Button11
				},
				new InputControlMapping {
					Handle = "System",
					Target = InputControlTarget.System,
					Source = Button12
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
					Source = Analog5,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Left",
					Target = InputControlTarget.DPadLeft,
					Source = Analog8,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Right",
					Target = InputControlTarget.DPadRight,
					Source = Analog8,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Up",
					Target = InputControlTarget.DPadUp,
					Source = Analog9,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Down",
					Target = InputControlTarget.DPadDown,
					Source = Analog9,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				},
				new InputControlMapping {
					Handle = "Tilt X",
					Target = InputControlTarget.TiltX,
					Source = Analog3
				},
				new InputControlMapping {
					Handle = "Tilt Y",
					Target = InputControlTarget.TiltY,
					Source = Analog4
				}
			};
		}
	}
}

