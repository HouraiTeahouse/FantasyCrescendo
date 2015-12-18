using System;


namespace InControl
{
	[AutoDiscover]
	public class GoogleNexusPlayerRemoteProfile : UnityInputDeviceProfile
	{
		public GoogleNexusPlayerRemoteProfile()
		{
			Name = "Google Nexus Player Remote";
			Meta = "Google Nexus Player Remote";

			SupportedPlatforms = new[] {
				"Android"
			};

			JoystickNames = new[] {
				"Google Nexus Remote"
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "A",
					Target = InputControlTarget.Action1,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "Back",
					Target = InputControlTarget.Back,
					Source = KeyCodeButton( UnityEngine.KeyCode.Escape )
				}
			};

			AnalogMappings = new[] {
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
					TargetRange = InputControlMapping.Range.Positive
				},
			};
		}
	}
}
