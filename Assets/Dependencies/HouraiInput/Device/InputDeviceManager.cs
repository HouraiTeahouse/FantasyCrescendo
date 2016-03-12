using System.Collections.Generic;

namespace InControl {
    public class InputDeviceManager {
        protected List<InputDevice> devices = new List<InputDevice>();


        public virtual void Update(ulong updateTick, float deltaTime) {
        }
    }
}