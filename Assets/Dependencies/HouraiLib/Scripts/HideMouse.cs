using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> Hides the cursor </summary>
    public class HideMouse : MonoBehaviour {

        /// <summary> Whether the cursor hiding works in the editor or not. </summary>
        [SerializeField]
        bool _inEditor;

#if UNITY_EDITOR
        void OnEnable() {
            if (_inEditor)
                Cursor.visible = false;
        }

        void OnDisable() {
            if (_inEditor)
                Cursor.visible = true;
        }
#else
        void OnEnable() {
            Cursor.visible = false;
        }

        void OnDisable() {
            Cursor.visible = true;
        }
#endif
    }

}