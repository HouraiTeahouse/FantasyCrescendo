#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


namespace InControl {
    public class XInputDevice : InputDevice {
        public int DeviceIndex { get; private set; }
        GamePadState state;


        public XInputDevice(int deviceIndex)
            : base("XInput Controller") {
            DeviceIndex = deviceIndex;
            SortOrder = deviceIndex;

            Meta = "XInput Controller #" + deviceIndex;

            AddControl(InputControlTarget.LeftStickX, "LeftStickX");
            AddControl(InputControlTarget.LeftStickY, "LeftStickY");
            AddControl(InputControlTarget.RightStickX, "RightStickX");
            AddControl(InputControlTarget.RightStickY, "RightStickY");

            AddControl(InputControlTarget.LeftTrigger, "LeftTrigger");
            AddControl(InputControlTarget.RightTrigger, "RightTrigger");

            AddControl(InputControlTarget.DPadUp, "DPadUp");
            AddControl(InputControlTarget.DPadDown, "DPadDown");
            AddControl(InputControlTarget.DPadLeft, "DPadLeft");
            AddControl(InputControlTarget.DPadRight, "DPadRight");

            AddControl(InputControlTarget.Action1, "Action1");
            AddControl(InputControlTarget.Action2, "Action2");
            AddControl(InputControlTarget.Action3, "Action3");
            AddControl(InputControlTarget.Action4, "Action4");

            AddControl(InputControlTarget.LeftBumper, "LeftBumper");
            AddControl(InputControlTarget.RightBumper, "RightBumper");

            AddControl(InputControlTarget.LeftStickButton, "LeftStickButton");
            AddControl(InputControlTarget.RightStickButton, "RightStickButton");

            AddControl(InputControlTarget.Start, "Start");
            AddControl(InputControlTarget.Back, "Back");

            QueryState();
        }


        public override void Update(ulong updateTick, float deltaTime) {
            QueryState();

            UpdateWithValue(InputControlTarget.LeftStickX, state.ThumbSticks.Left.X, updateTick);
            UpdateWithValue(InputControlTarget.LeftStickY, state.ThumbSticks.Left.Y, updateTick);
            UpdateWithValue(InputControlTarget.RightStickX, state.ThumbSticks.Right.X, updateTick);
            UpdateWithValue(InputControlTarget.RightStickY, state.ThumbSticks.Right.Y, updateTick);

            UpdateWithValue(InputControlTarget.LeftTrigger, state.Triggers.Left, updateTick);
            UpdateWithValue(InputControlTarget.RightTrigger, state.Triggers.Right, updateTick);

            UpdateWithState(InputControlTarget.DPadUp, state.DPad.Up == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.DPadDown, state.DPad.Down == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.DPadLeft, state.DPad.Left == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.DPadRight, state.DPad.Right == ButtonState.Pressed, updateTick);

            UpdateWithState(InputControlTarget.Action1, state.Buttons.A == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.Action2, state.Buttons.B == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.Action3, state.Buttons.X == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.Action4, state.Buttons.Y == ButtonState.Pressed, updateTick);

            UpdateWithState(InputControlTarget.LeftBumper, state.Buttons.LeftShoulder == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.RightBumper, state.Buttons.RightShoulder == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputControlTarget.LeftStickButton, state.Buttons.LeftStick == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputControlTarget.RightStickButton, state.Buttons.RightStick == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputControlTarget.Start, state.Buttons.Start == ButtonState.Pressed, updateTick);
            UpdateWithState(InputControlTarget.Back, state.Buttons.Back == ButtonState.Pressed, updateTick);
        }


        public override void Vibrate(float leftMotor, float rightMotor) {
            GamePad.SetVibration((PlayerIndex) DeviceIndex, leftMotor, rightMotor);
        }


        void QueryState() {
            state = GamePad.GetState((PlayerIndex) DeviceIndex, GamePadDeadZone.Circular);
        }


        public bool IsConnected {
            get { return state.IsConnected; }
        }
    }
}

#endif