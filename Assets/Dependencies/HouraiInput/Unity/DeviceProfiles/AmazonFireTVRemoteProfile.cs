using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class AmazonFireTVRemote : UnityInputDeviceProfile
	{
		public AmazonFireTVRemote()
		{
			Name = "Amazon Fire TV Remote";
			Meta = "Amazon Fire TV Remote on Amazon Fire TV";

			SupportedPlatforms = new[] {
				"Amazon AFTB",
				"Amazon AFTM"
			};

			JoystickNames = new[] {
				"",
				"Amazon Fire TV Remote"
			};

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "A",
					Target = InputControlTarget.Action1,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "Back",
					Target = InputControlTarget.Select,
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
					TargetRange = InputControlMapping.Range.Positive,
				}
			};
		}
	}
}

