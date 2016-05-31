using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


namespace HouraiTeahouse.HouraiInput {
    public sealed class AutoDiscover : Attribute {
    }


    public class UnityInputDeviceProfile {
        public string Name { get; protected set; }
        public string Meta { get; protected set; }

        public InputMapping[] AnalogMappings { get; protected set; }
        public InputMapping[] ButtonMappings { get; protected set; }

        protected string[] SupportedPlatforms;
        protected string[] JoystickNames;
        protected string[] JoystickRegex;

        protected string LastResortRegex;

        private static readonly HashSet<Type> HiddenTypes = new HashSet<Type>();

        float sensitivity;
        float lowerDeadZone;
        float upperDeadZone;


        public UnityInputDeviceProfile() {
            Name = "";
            Meta = "";

            sensitivity = 1.0f;
            lowerDeadZone = 0.2f;
            upperDeadZone = 0.9f;

            AnalogMappings = new InputMapping[0];
            ButtonMappings = new InputMapping[0];
        }

        public float Sensitivity {
            get { return sensitivity; }
            protected set { sensitivity = Mathf.Clamp01(value); }
        }

        public float LowerDeadZone {
            get { return lowerDeadZone; }
            protected set { lowerDeadZone = Mathf.Clamp01(value); }
        }

        public float UpperDeadZone {
            get { return upperDeadZone; }
            protected set { upperDeadZone = Mathf.Clamp01(value); }
        }

        public bool IsSupportedOnThisPlatform {
            get {
                if (SupportedPlatforms == null || SupportedPlatforms.Length == 0)
                    return true;
                return SupportedPlatforms.Any(platform => HInput.Platform.Contains(platform.ToUpper()));
            }
        }

        public bool IsJoystick {
            get {
                return (LastResortRegex != null) ||
                       !JoystickNames.IsNullOrEmpty() ||
                       !JoystickRegex.IsNullOrEmpty();
            }
        }

        public bool HasJoystickName(string joystickName) {
            if (!IsJoystick)
                return false;
            if (JoystickNames != null && JoystickNames.Contains(joystickName, StringComparer.OrdinalIgnoreCase)) 
                return true;
            return JoystickRegex != null && JoystickRegex.Any(t => Regex.IsMatch(joystickName, t, RegexOptions.IgnoreCase));
        }


        public bool HasLastResortRegex(string joystickName) {
            if (!IsJoystick)
                return false;
            return LastResortRegex != null && Regex.IsMatch(joystickName, LastResortRegex, RegexOptions.IgnoreCase);
        }

        public bool HasJoystickOrRegexName(string joystickName) {
            return HasJoystickName(joystickName) || HasLastResortRegex(joystickName);
        }

        public static void Hide(Type type) {
            HiddenTypes.Add(type);
        }

        public bool IsHidden {
            get { return HiddenTypes.Contains(GetType()); }
        }

        public virtual bool IsKnown {
            get { return true; }
        }

        public int AnalogCount {
            get { return AnalogMappings.Length; }
        }

        public int ButtonCount {
            get { return ButtonMappings.Length; }
        }
        #region InputSource Helpers

        protected static InputSource Button(int index) {
            return new UnityButtonSource(index);
        }

        protected static InputSource Analog(int index) {
            return new UnityAnalogSource(index);
        }

        protected static InputSource KeyCodeButton(KeyCode keyCodeList) {
            return new UnityKeyCodeSource(keyCodeList);
        }

        protected static InputSource KeyCodeComboButton(params KeyCode[] keyCodeList) {
            return new UnityKeyCodeComboSource(keyCodeList);
        }

        protected static InputSource KeyCodeAxis(KeyCode negativeKeyCode, KeyCode positiveKeyCode) {
            return new UnityKeyCodeAxisSource(negativeKeyCode, positiveKeyCode);
        }

        protected static InputSource Button0 = Button(0);
        protected static InputSource Button1 = Button(1);
        protected static InputSource Button2 = Button(2);
        protected static InputSource Button3 = Button(3);
        protected static InputSource Button4 = Button(4);
        protected static InputSource Button5 = Button(5);
        protected static InputSource Button6 = Button(6);
        protected static InputSource Button7 = Button(7);
        protected static InputSource Button8 = Button(8);
        protected static InputSource Button9 = Button(9);
        protected static InputSource Button10 = Button(10);
        protected static InputSource Button11 = Button(11);
        protected static InputSource Button12 = Button(12);
        protected static InputSource Button13 = Button(13);
        protected static InputSource Button14 = Button(14);
        protected static InputSource Button15 = Button(15);
        protected static InputSource Button16 = Button(16);
        protected static InputSource Button17 = Button(17);
        protected static InputSource Button18 = Button(18);
        protected static InputSource Button19 = Button(19);

        protected static InputSource Analog0 = Analog(0);
        protected static InputSource Analog1 = Analog(1);
        protected static InputSource Analog2 = Analog(2);
        protected static InputSource Analog3 = Analog(3);
        protected static InputSource Analog4 = Analog(4);
        protected static InputSource Analog5 = Analog(5);
        protected static InputSource Analog6 = Analog(6);
        protected static InputSource Analog7 = Analog(7);
        protected static InputSource Analog8 = Analog(8);
        protected static InputSource Analog9 = Analog(9);
        protected static InputSource Analog10 = Analog(10);
        protected static InputSource Analog11 = Analog(11);
        protected static InputSource Analog12 = Analog(12);
        protected static InputSource Analog13 = Analog(13);
        protected static InputSource Analog14 = Analog(14);
        protected static InputSource Analog15 = Analog(15);
        protected static InputSource Analog16 = Analog(16);
        protected static InputSource Analog17 = Analog(17);
        protected static InputSource Analog18 = Analog(18);
        protected static InputSource Analog19 = Analog(19);

        protected static InputSource MouseButton0 = new UnityMouseButtonSource(0);
        protected static InputSource MouseButton1 = new UnityMouseButtonSource(1);
        protected static InputSource MouseButton2 = new UnityMouseButtonSource(2);

        protected static InputSource MouseXAxis = new UnityMouseAxisSource("x");
        protected static InputSource MouseYAxis = new UnityMouseAxisSource("y");
        protected static InputSource MouseScrollWheel = new UnityMouseAxisSource("z");

        #endregion
    }
}
