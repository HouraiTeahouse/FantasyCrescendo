using System;
using System.Collections;
using UnityEngine;
using InControl;


namespace CustomProfileExample
{
	// This custom profile is enabled by adding it to the Custom Profiles list
	// on the InControlManager component, or you can attach it yourself like so:
	// InputManager.AttachDevice( new UnityInputDevice( "KeyboardAndMouseProfile" ) );
	// 
	public class KeyboardAndMouseProfile : UnityInputDeviceProfile
	{
		public KeyboardAndMouseProfile()
		{
			Name = "Keyboard/Mouse";
			Meta = "A keyboard and mouse combination profile appropriate for FPS.";

			// This profile only works on desktops.
			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			LowerDeadZone = 0.0f;
			UpperDeadZone = 1.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Fire - Mouse",
					Target = InputControlTarget.Action1,
					Source = MouseButton0
				},
				new InputControlMapping
				{
					Handle = "Fire - Keyboard",
					Target = InputControlTarget.Action1,
					// KeyCodeButton fires when any of the provided KeyCode params are down.
					Source = KeyCodeButton( KeyCode.F, KeyCode.Return )
				},
				new InputControlMapping
				{
					Handle = "AltFire",
					Target = InputControlTarget.Action2,
					Source = MouseButton2
				},
				new InputControlMapping
				{
					Handle = "Middle",
					Target = InputControlTarget.Action3,
					Source = MouseButton1
				},
				new InputControlMapping
				{
					Handle = "Jump",
					Target = InputControlTarget.Action4,
					Source = KeyCodeButton( KeyCode.Space )
				},
				new InputControlMapping
				{
					Handle = "Combo",
					Target = InputControlTarget.LeftBumper,
					// KeyCodeComboButton requires that all KeyCode params are down simultaneously.
					Source = KeyCodeComboButton( KeyCode.LeftAlt, KeyCode.Alpha1 )
				},
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Move X",
					Target = InputControlTarget.LeftStickX,
					// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
					Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
				},
				new InputControlMapping
				{
					Handle = "Move Y",
					Target = InputControlTarget.LeftStickY,
					// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
					Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
				},
				new InputControlMapping {
					Handle = "Move X Alternate",
					Target = InputControlTarget.LeftStickX,
					Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
				},
				new InputControlMapping {
					Handle = "Move Y Alternate",
					Target = InputControlTarget.LeftStickY,
					Source = KeyCodeAxis( KeyCode.DownArrow, KeyCode.UpArrow )
				},
				new InputControlMapping
				{
					Handle = "Look X",
					Target = InputControlTarget.RightStickX,
					Source = MouseXAxis,
					Raw    = true,
					Scale  = 0.1f
				},
				new InputControlMapping
				{
					Handle = "Look Y",
					Target = InputControlTarget.RightStickY,
					Source = MouseYAxis,
					Raw    = true,
					Scale  = 0.1f
				},
				new InputControlMapping
				{
					Handle = "Look Z",
					Target = InputControlTarget.ScrollWheel,
					Source = MouseScrollWheel,
					Raw    = true,
					Scale  = 0.1f
				}
			};
		}
	}
}

