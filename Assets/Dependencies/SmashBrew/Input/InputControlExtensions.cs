using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.HouraiInput;

namespace HouraiTeahouse.SmashBrew {

    public static class InputControlExtensions {

        public static IEnumerable<InputControl> GetControl(this IEnumerable<InputDevice> devices, InputTarget target) {
            return devices.Select(d => d[target]);
        }

        public static IEnumerable<InputControl> GetControls(this InputDevice device, IEnumerable<InputTarget> targets) {
            return targets.Select<InputTarget, InputControl>(device.GetControl);
        }

        public static IEnumerable<InputControl> GetControls(this IEnumerable<InputDevice> devices,
            IEnumerable<InputTarget> targets) {
            return devices.SelectMany(d => d.GetControls(targets));
        }

    }

}
