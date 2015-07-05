using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    protected virtual void Awake() {
#if DEBUG
        //Do nothing, these scripts should be in a Debug system
#else
        //Not a Debug Build. Remove immediately.
        Destroy(this);
#endif
    }

}
