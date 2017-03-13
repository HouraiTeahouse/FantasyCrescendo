using System.Collections;
using System.Collections.Generic;
using HouraiTeahouse.SmashBrew;
using UnityEngine;

namespace HouraiTeahouse {

public class HitobxDisplay : MonoBehaviour {

    [SerializeField]
    KeyCode _key = KeyCode.F11;

    /// <summary> Unity callback. Called once per frame. </summary>
    void Update() {
        if (Input.GetKeyDown(_key))
            Hitbox.DrawHitboxes = !Hitbox.DrawHitboxes;
    }

}

}

