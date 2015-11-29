using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class InputManager : MonoBehaviour {

        [SerializeField]
        private string[] axes;

        [SerializeField]
        private string[] buttons;

        private static InputManager _isntance;

        public static InputManager Instance {
            get { return _isntance; }
        }

        private IInputController[] _controllers;

        void Awake() {
            if(Instance != null)
                Destroy(Instance);
            _isntance = this;
            _controllers = new IInputController[Config.Instance.MaxPlayers];
            for(var i = 0; i < _controllers.Length; i++)
                _controllers[i] = new TestController(i + 1, axes, buttons);
        }

        public IInputController GetController(int controllerNum) {
            if (controllerNum < 0 || controllerNum >= _controllers.Length)
                return null;
            return _controllers[controllerNum];
        }

    }

    internal class TestController : IInputController {

        private readonly Dictionary<string, IInputAxis> _axes;
        private readonly Dictionary<string, IInputButton> _buttons;  

        internal TestController(int player, string[] axes, string[] buttons) {
            _axes = new Dictionary<string, IInputAxis>();
            _buttons = new Dictionary<string, IInputButton>();
            foreach (var axis in axes)
                _axes[axis] = new UnityInput(string.Format("Player {0} {1}", player, axis));
            foreach(var button in buttons)
                _buttons[button] = new UnityInput(string.Format("Player {0} {1}", player, button));
        }

        public IEnumerable<string> AxisNames {
            get { return _axes.Keys; }
        }

        public IEnumerable<string> ButtonNames {
            get { return _buttons.Keys; }
        }

        public IInputAxis GetAxis(string axisName) {
            return _axes[axisName];
        }

        public IInputButton GetButton(string buttonName) {
            return _buttons[buttonName];
        }

    }

    internal class UnityInput : IInputButton, IInputAxis {

        private readonly string _name;

        public string Name {
            get { return _name; }
        }

        public UnityInput(string name) {
            _name = name;
        }

        public bool GetButtonValue() {
            return Input.GetButtonDown(_name);
        }

        public float GetAxisValue() {
            return Input.GetAxisRaw(_name);
        }

    }

}