using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class ExecutionerXProfile : UnityInputDeviceProfile
	{
		public ExecutionerXProfile()
		{
			Name = "Executioner X Controller";
			Meta = "Executioner X Controller";

			SupportedPlatforms = new[] {
				"Windows",
				"OS X"
			};

			JoystickNames = new[] {
				"Zeroplus PS Vibration Feedback Converter",
				"Zeroplus PS Vibration Feedback Converter "
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "3",
					Target = InputControlTarget.Action1,
					Source = Button2
				},
				new InputControlMapping {
					Handle = "2",
					Target = InputControlTarget.Action2,
					Source = Button1
				},
				new InputControlMapping {
					Handle = "4",
					Target = InputControlTarget.Action3,
					Source = Button3
				},
				new InputControlMapping {
					Handle = "1",
					Target = InputControlTarget.Action4,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "Left Bumper",
					Target = InputControlTarget.LeftBumper,
					Source = Button6
				},
				new InputControlMapping {
					Handle = "Right Bumper",
					Target = InputControlTarget.RightBumper,
					Source = Button7
				},
				new InputControlMapping {
					Handle = "Start",
					Target = InputControlTarget.Start,
					Source = Button11
				},
				new InputControlMapping {
					Handle = "Options",
					Target = InputControlTarget.Select,
					Source = Button8
				},
				new InputControlMapping {
					Handle = "Left Trigger",
					Target = InputControlTarget.LeftTrigger,
					Source = Button4
				},
				new InputControlMapping {
					Handle = "Right Trigger",
					Target = InputControlTarget.RightTrigger,
					Source = Button5
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
					Source = Analog6,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Right",
					Target = InputControlTarget.DPadRight,
					Source = Analog6,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Down",
					Target = InputControlTarget.DPadDown,
					Source = Analog7,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Up",
					Target = InputControlTarget.DPadUp,
					Source = Analog7,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				}
			};
		}
	}
}

