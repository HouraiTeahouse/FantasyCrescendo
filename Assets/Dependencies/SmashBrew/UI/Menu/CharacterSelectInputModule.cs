using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {

    public class CharacterSelectInputModule : PointerInputModule {

        [SerializeField] private InputControlTarget _horizontal = InputControlTarget.LeftStickX;

        [SerializeField] private InputControlTarget _vertical = InputControlTarget.LeftStickY;

        [SerializeField]
        private InputControlTarget _submit = InputControlTarget.Action1;

        [SerializeField] private InputControlTarget _cancel = InputControlTarget.Action2;

        [SerializeField] private InputControlTarget _changeLeft = InputControlTarget.DPadLeft;

        [SerializeField] private InputControlTarget _changeRight = InputControlTarget.DPadRight;

        private List<PlayerPointer> _pointers;
        private PointerEventData _eventData;

        internal static CharacterSelectInputModule Instance { get; private set; }

        internal void AddPointer(PlayerPointer pointer) {
            if(pointer)
                _pointers.Add(pointer);
        }

        internal void RemovePointer(PlayerPointer pointer) {
            _pointers.Remove(pointer);
        }

        protected override void Awake() {
            base.Awake();
            Instance = this;
            _pointers = new List<PlayerPointer>();
        }

        public override void Process() {
            for (var i = 0; i < _pointers.Count; i++) {
                PlayerPointer pointer = _pointers[i];
                Player player = pointer.Player;
                InputDevice controller = player.Controller;
                if (controller == null)
                    continue;
                // Move the controller
                pointer.Move(new Vector2(controller.GetControl(_horizontal), controller.GetControl(_vertical)));
                ProcessPointerSubmit(pointer, i, controller);
                CharacterChange(pointer, player, controller);
            }
        }

        void ProcessPointerSubmit(PlayerPointer pointer, int i, InputDevice controller) {
            GetPointerData(i, out _eventData, true);
            _eventData.position = Camera.main.WorldToScreenPoint(pointer.transform.position);
            EventSystem.current.RaycastAll(_eventData, m_RaycastResultCache);
            RaycastResult result = FindFirstRaycast(m_RaycastResultCache);
            ProcessMove(_eventData);
            bool success = false;
            _eventData.clickCount = 0;
            if (controller.GetControl(_submit).WasPressed) {
                _eventData.pressPosition = _eventData.position;
                _eventData.clickCount = 1;
                _eventData.clickTime = Time.unscaledTime;
                _eventData.pointerPressRaycast = result;
                if (m_RaycastResultCache.Count > 0) {
                    _eventData.selectedObject = result.gameObject;
                    _eventData.pointerPress = ExecuteEvents.ExecuteHierarchy(result.gameObject, _eventData,
                        ExecuteEvents.submitHandler);
                    _eventData.rawPointerPress = result.gameObject;
                    success = true;
                }
            }
            if (!success) {
                _eventData.clickCount = 0;
                _eventData.eligibleForClick = false;
                _eventData.pointerPress = null;
                _eventData.rawPointerPress = null;
            }
        }

        void CharacterChange(PlayerPointer pointer, Player player, InputDevice controller) {
            if (!player.SelectedCharacter)
                return;
            if (controller.GetControl(_changeLeft).WasPressed) {
                player.Pallete--;
            }
            if (controller.GetControl(_changeRight).WasPressed) {
                player.Pallete++;
            }
        }
    }
}
