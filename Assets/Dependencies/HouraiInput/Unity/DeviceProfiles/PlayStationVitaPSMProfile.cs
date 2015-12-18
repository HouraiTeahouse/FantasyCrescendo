using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class PlayStationVitaPSMProfile : UnityInputDeviceProfile
	{
		public PlayStationVitaPSMProfile()
		{
			Name = "PlayStation Mobile";
			Meta = "PlayStation Mobile on Vita";

			SupportedPlatforms = new[] {
				"PSM UNITY FOR PSM",
				"PSM ON PS VITA",
				"PS VITA",
				"PSP2OS"
			};

			JoystickNames = new[] {
				"PS Vita"
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "Cross",
					Target = InputControlTarget.Action1,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "Circle",
					Target = InputControlTarget.Action2,
					Source = Button1
				},
				new InputControlMapping {
					Handle = "Square",
					Target = InputControlTarget.Action3,
					Source = Button2
				},
				new InputControlMapping {
					Handle = "Triangle",
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
					Handle = "Select",
					Target = InputControlTarget.Select,
					Source = Button6
				},
				new InputControlMapping {
					Handle = "Start",
					Target = InputControlTarget.Start,
					Source = Button7
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
					Source = Analog3
				},
				new InputControlMapping {
					Handle = "Right Stick Y",
					Target = InputControlTarget.RightStickY,
					Source = Analog4,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Left",
					Target = InputControlTarget.DPadLeft,
					Source = Analog5,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				},
				new InputControlMapping {
					Handle = "DPad Right",
					Target = InputControlTarget.DPadRight,
					Source = Analog5,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Up",
					Target = InputControlTarget.DPadUp,
					Source = Analog6,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping {
					Handle = "DPad Down",
					Target = InputControlTarget.DPadDown,
					Source = Analog6,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				}
			};
		}
	}
}
