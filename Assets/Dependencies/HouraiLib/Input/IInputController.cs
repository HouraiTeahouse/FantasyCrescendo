using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hourai {

    public interface IInputController {

        IEnumerable<string> AxisNames { get; }

        IEnumerable<string> ButtonNames { get; }

        IInputAxis GetAxis(string axisName);

        IInputButton GetButton(string buttonName);

    }

}