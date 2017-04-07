using System;
using System.IO;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> Takes screenshots upon pressing F12 </summary>
    //TODO: Generalize
    public class Screenshot : MonoBehaviour {

        [SerializeField]
        string _dateTimeFormat = "MM-dd-yyyy-HHmmss";

        [SerializeField]
        string _format = "screenshot-{0}";

        [SerializeField]
        KeyCode _key = KeyCode.F12;

        /// <summary> Unity callback. Called once per frame. </summary>
        void Update() {
            if (!Input.GetKeyDown(_key))
                return;
            string filename = _format.With(DateTime.UtcNow.ToString(_dateTimeFormat)) + ".png";
            string path = Path.Combine(Application.dataPath, filename);

            Log.GetLogger(this).Info("Screenshot taken. Saved to {0}", path);

            if (File.Exists(path))
                File.Delete(path);

            Application.CaptureScreenshot(Application.platform == RuntimePlatform.IPhonePlayer ? filename : path);
        }

    }

}
