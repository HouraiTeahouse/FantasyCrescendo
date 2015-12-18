using System;


namespace InControl
{
	// @cond nodoc
	[AutoDiscover]
	public class AppleMFiProfile : UnityInputDeviceProfile
	{
		public AppleMFiProfile()
		{
			Name = "Apple MFi Controller";
			Meta = "Apple MFi Controller on iOS";

			SupportedPlatforms = new[] {
				"iPhone"
			};

			LastResortRegex = ""; // Match anything.

			LowerDeadZone = 0.05f;
			UpperDeadZone = 0.95f;

			ButtonMappings = new[] {
				new InputControlMapping {
					Handle = "A",
					Target = InputControlTarget.Action1,
					Source = Button14
				},
				new InputControlMapping {
					Handle = "B",
					Target = InputControlTarget.Action2,
					Source = Button13
				},
				new InputControlMapping {
					Handle = "X",
					Target = InputControlTarget.Action3,
					Source = Button15
				},
				new InputControlMapping {
					Handle = "Y",
					Target = InputControlTarget.Action4,
					Source = Button12
				},
				new InputControlMapping {
					Handle = "DPad Up",
					Target = InputControlTarget.DPadUp,
					Source = Button4
				},
				new InputControlMapping {
					Handle = "DPad Down",
					Target = InputControlTarget.DPadDown,
					Source = Button6
				},
				new InputControlMapping {
					Handle = "DPad Left",
					Target = InputControlTarget.DPadLeft,
					Source = Button7
				},
				new InputControlMapping {
					Handle = "DPad Right",
					Target = InputControlTarget.DPadRight,
					Source = Button5
				},
				new InputControlMapping {
					Handle = "Left Bumper",
					Target = InputControlTarget.LeftBumper,
					Source = Button8
				},
				new InputControlMapping {
					Handle = "Right Bumper",
					Target = InputControlTarget.RightBumper,
					Source = Button9
				},
				new InputControlMapping {
					Handle = "Pause",
					Target = InputControlTarget.Pause,
					Source = Button0
				},
				new InputControlMapping {
					Handle = "Left Trigger",
					Target = InputControlTarget.LeftTrigger,
					Source = Button10
				},
				new InputControlMapping {
					Handle = "Right Trigger",
					Target = InputControlTarget.RightTrigger,
					Source = Button11
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
				}
			};
		}
	}
}

