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

using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {
    public class SmashBrewInputModule : PointerInputModule {
        [SerializeField]
        InputTarget _cancel = InputTarget.Action2;

        float _currentDelay;

        [SerializeField]
        float _deadZone = 0.1f;

        [SerializeField]
        InputTarget _horizontal = InputTarget.LeftStickX;

        [SerializeField]
        float _navigationDelay = 0.25f;

        [SerializeField]
        InputTarget _submit = InputTarget.Action1;

        [SerializeField]
        InputTarget _vertical = InputTarget.LeftStickY;

        /// <summary> Called when the InputModule is activated. </summary>
        public override void ActivateModule() {
            base.ActivateModule();

            if (!eventSystem.currentSelectedGameObject)
                eventSystem.SetSelectedGameObject(
                    eventSystem.firstSelectedGameObject,
                    GetBaseEventData());
        }

        /// <summary> Called when the InputModule is deactivated. </summary>
        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        void SendMessage<T>(InputTarget targetControl,
                            GameObject target,
                            ExecuteEvents.EventFunction<T> handler)
            where T : IEventSystemHandler {
            int count = HInput.Devices.Count;
            for (var i = 0; i < count; i++) {
                InputDevice device = HInput.Devices[i];
                if (!device[targetControl].WasPressed)
                    continue;
                ExecuteEvents.Execute(target, GetBaseEventData(), handler);
                return;
            }
        }

        /// <summary> Called once every frame while InputModule is active. </summary>
        public override void Process() {
            if (!eventSystem)
                return;
            GameObject target = eventSystem.currentSelectedGameObject;

            SendMessage(_submit, target, ExecuteEvents.submitHandler);
            SendMessage(_cancel, target, ExecuteEvents.cancelHandler);

            _currentDelay -= Time.deltaTime;
            if (!eventSystem.sendNavigationEvents || _currentDelay >= 0)
                return;

            Vector2 axis = Vector2.zero;
            int count = HInput.Devices.Count;
            for (var i = 0; i < count; i++) {
                InputDevice device = HInput.Devices[i];
                axis.x += device[_horizontal];
                axis.y += device[_vertical];
            }
            axis /= HInput.Devices.Count;

            AxisEventData moveData = GetAxisEventData(axis.x, axis.y, _deadZone);
            ExecuteEvents.Execute(target, moveData, ExecuteEvents.moveHandler);
            if (moveData.moveDir != MoveDirection.None)
                _currentDelay = _navigationDelay;
        }
    }
}