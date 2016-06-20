// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using XInputDotNetPure;

namespace HouraiTeahouse.HouraiInput {
    public class XInputDevice : InputDevice {
        GamePadState state;


        public XInputDevice(int deviceIndex) : base("XInput Controller") {
            DeviceIndex = deviceIndex;
            SortOrder = deviceIndex;

            Meta = "XInput Controller #" + deviceIndex;

            AddControl(InputTarget.LeftStickX, "LeftStickX");
            AddControl(InputTarget.LeftStickY, "LeftStickY");
            AddControl(InputTarget.RightStickX, "RightStickX");
            AddControl(InputTarget.RightStickY, "RightStickY");

            AddControl(InputTarget.LeftTrigger, "LeftTrigger");
            AddControl(InputTarget.RightTrigger, "RightTrigger");

            AddControl(InputTarget.DPadUp, "DPadUp");
            AddControl(InputTarget.DPadDown, "DPadDown");
            AddControl(InputTarget.DPadLeft, "DPadLeft");
            AddControl(InputTarget.DPadRight, "DPadRight");

            AddControl(InputTarget.Action1, "Action1");
            AddControl(InputTarget.Action2, "Action2");
            AddControl(InputTarget.Action3, "Action3");
            AddControl(InputTarget.Action4, "Action4");

            AddControl(InputTarget.LeftBumper, "LeftBumper");
            AddControl(InputTarget.RightBumper, "RightBumper");

            AddControl(InputTarget.LeftStickButton, "LeftStickButton");
            AddControl(InputTarget.RightStickButton, "RightStickButton");

            AddControl(InputTarget.Start, "Start");
            AddControl(InputTarget.Back, "Back");

            QueryState();
        }

        public int DeviceIndex { get; private set; }


        public bool IsConnected {
            get { return state.IsConnected; }
        }


        public override void Update(ulong updateTick, float deltaTime) {
            QueryState();

            UpdateWithValue(InputTarget.LeftStickX,
                state.ThumbSticks.Left.X,
                updateTick);
            UpdateWithValue(InputTarget.LeftStickY,
                state.ThumbSticks.Left.Y,
                updateTick);
            UpdateWithValue(InputTarget.RightStickX,
                state.ThumbSticks.Right.X,
                updateTick);
            UpdateWithValue(InputTarget.RightStickY,
                state.ThumbSticks.Right.Y,
                updateTick);

            UpdateWithValue(InputTarget.LeftTrigger,
                state.Triggers.Left,
                updateTick);
            UpdateWithValue(InputTarget.RightTrigger,
                state.Triggers.Right,
                updateTick);

            UpdateWithState(InputTarget.DPadUp,
                state.DPad.Up == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.DPadDown,
                state.DPad.Down == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.DPadLeft,
                state.DPad.Left == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.DPadRight,
                state.DPad.Right == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputTarget.Action1,
                state.Buttons.A == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.Action2,
                state.Buttons.B == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.Action3,
                state.Buttons.X == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.Action4,
                state.Buttons.Y == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputTarget.LeftBumper,
                state.Buttons.LeftShoulder == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.RightBumper,
                state.Buttons.RightShoulder == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputTarget.LeftStickButton,
                state.Buttons.LeftStick == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.RightStickButton,
                state.Buttons.RightStick == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputTarget.Start,
                state.Buttons.Start == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.Back,
                state.Buttons.Back == ButtonState.Pressed,
                updateTick);
        }


        public override void Vibrate(float leftMotor, float rightMotor) {
            GamePad.SetVibration((PlayerIndex) DeviceIndex,
                leftMotor,
                rightMotor);
        }


        void QueryState() {
            state = GamePad.GetState((PlayerIndex) DeviceIndex,
                GamePadDeadZone.Circular);
        }
    }
}

#endif