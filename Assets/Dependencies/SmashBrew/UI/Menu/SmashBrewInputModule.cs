using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {
    public class SmashBrewInputModule : PointerInputModule {
        [SerializeField] private string _horizontalKeyboard = "horizontal";

        [SerializeField] private string _verticalKeyboard = "vertical";

        [SerializeField] private string _submitKeyboard = "submit";

        [SerializeField] private string _cancelKeyboard = "cancel";

        [SerializeField] private InputTarget _horizontalGamepad = InputTarget.LeftStickX;

        [SerializeField] private InputTarget _verticalGamepad = InputTarget.LeftStickY;

        [SerializeField] private InputTarget _submitGamepad = InputTarget.Action1;

        [SerializeField] private InputTarget _cancelGamepad = InputTarget.Action2;

        [SerializeField] private float _deadZone = 0.1f;

        [SerializeField] private float _navigationDelay = 0.25f;

        private float currentDelay;

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

        /// <summary>
        /// Called once every frame while InputModule is active.
        /// </summary>
        public override void Process() {
            if (!eventSystem)
                return;
            GameObject target = eventSystem.currentSelectedGameObject;

            bool submit = Input.GetButtonDown(_submitKeyboard);
            bool cancel = Input.GetButtonDown(_cancelKeyboard);

            float x = Input.GetAxisRaw(_horizontalKeyboard);
            float y = Input.GetAxisRaw(_verticalKeyboard);
            var count = 1;
            foreach (InputDevice device in HInput.Devices) {
                if (device == null)
                    continue;
                x += device.GetControl(_horizontalGamepad);
                y += device.GetControl(_verticalGamepad);
                submit |= device.GetControl(_submitGamepad).WasPressed;
                cancel |= device.GetControl(_cancelGamepad).WasPressed;
                count++;
            }

            if (submit)
                ExecuteEvents.Execute(target, GetBaseEventData(), ExecuteEvents.submitHandler);
            if (cancel)
                ExecuteEvents.Execute(target, GetBaseEventData(), ExecuteEvents.cancelHandler);

            currentDelay -= Time.deltaTime;
            if (!eventSystem.sendNavigationEvents || currentDelay >= 0)
                return;

            x /= count;
            y /= count;
            AxisEventData moveData = GetAxisEventData(x, y, _deadZone);
            ExecuteEvents.Execute(target, moveData, ExecuteEvents.moveHandler);
            if (moveData.moveDir != MoveDirection.None)
                currentDelay = _navigationDelay;
        }
    }
}
