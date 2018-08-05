using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public enum StateEntryPolicy {
  Normal,             // Any other state can pass into it
  Passthrough,        // State can be transitioned to, but will instantly be reevaluated for an exit
  Blocked             // State cannot be transitioned to
}

}
