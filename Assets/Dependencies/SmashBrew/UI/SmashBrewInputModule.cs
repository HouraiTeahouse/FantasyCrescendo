using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hourai.SmashBrew.UI {

    public class SmashBrewInputModule : BaseInputModule {

        [SerializeField]
        private InputControlTarget _horizontal = InputControlTarget.LeftStickX;

        [SerializeField]
        private InputControlTarget _vertical = InputControlTarget.LeftStickY;

        [SerializeField]
        private InputControlTarget _submit = InputControlTarget.Action1;

        [SerializeField]
        private InputControlTarget _cancel = InputControlTarget.Action2;

        [SerializeField]
        private float _deadZone = 0.1f;

        /// <summary>
        /// Called once every frame while InputModule is active.
        /// </summary>
        public override void Process() {
            if (!eventSystem)
                return;

            GameObject target = eventSystem.currentSelectedGameObject;

            if (InputManager.Devices.Any(controller => controller.GetControl(_submit).WasPressed))
                ExecuteEvents.Execute(target, null, ExecuteEvents.submitHandler);
            if (InputManager.Devices.Any(controller => controller.GetControl(_cancel).WasPressed))
                ExecuteEvents.Execute(target, null, ExecuteEvents.cancelHandler);
            AxisEventData moveData = GetAxisEventData(InputManager.Devices.Average(c => c.GetControl(_horizontal).Value),
                                                      InputManager.Devices.Average(c => c.GetControl(_vertical).Value),
                                                      _deadZone);
            if (moveData.moveDir == MoveDirection.None)
                ExecuteEvents.Execute(target, moveData, ExecuteEvents.moveHandler);
        }

    }

}