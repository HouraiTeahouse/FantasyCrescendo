using UnityEngine;
using System.Collections;
using System.Security.Policy;

namespace Hourai.SmashBrew {
    
    public abstract class InputController {

        public IInputAxis Horizontal { get; protected set; }
        public IInputAxis Vertical { get; protected set; }
        public IInputButton Attack { get; protected set; }
        public IInputButton Special { get; protected set; }
        public IInputButton Shield { get; protected set; }
        public IInputButton Jump { get; protected set; }

    }

}

