using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {
    public class SmashBrewInputModule : PointerInputModule {
        [SerializeField]
        private InputTarget _horizontal = InputTarget.LeftStickX;

        [SerializeField]
        private InputTarget _vertical = InputTarget.LeftStickY;

        [SerializeField]
        private InputTarget _submit = InputTarget.Action1;

        [SerializeField]
        private InputTarget _cancel = InputTarget.Action2;

        [SerializeField]
        private float _deadZone = 0.1f;

        [SerializeField]
        private float _navigationDelay = 0.25f;

        private float _currentDelay;

        /// <summary>
        /// Called when the InputModule is activated.
        /// </summary>
        public override void ActivateModule() {
            base.ActivateModule();

            if (!eventSystem.currentSelectedGameObject)
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject, GetBaseEventData());
        }

        /// <summary>
        /// Called when the InputModule is deactivated.
        /// </summary>
        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        void SendMessage<T>(InputTarget targetControl, GameObject target, ExecuteEvents.EventFunction<T> handler) where T : IEventSystemHandler {
            int count = HInput.Devices.Count;
            for (var i = 0; i < count; i++) {
                InputDevice device = HInput.Devices[i];
                if (!device[targetControl].WasPressed) continue;
                ExecuteEvents.Execute(target, GetBaseEventData(), handler);
                return;
            }
        }

        /// <summary>
        /// Called once every frame while InputModule is active.
        /// </summary>
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
