using System;
using System.IO;
using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// Takes screenshots upon pressing F12
    /// </summary>
    //TODO: Generalize
    public class Screenshot : MonoBehaviour {

        [SerializeField]
        private KeyCode _key = KeyCode.F12;

        [SerializeField]
        private string _format = "screenshot-{0}";

        [SerializeField]
        private string _dateTimeFormat = "MM-dd-yyyy-HHmmss";

        /// <summary>
        /// Unity callback. Called once per frame.
        /// </summary>
        void Update() {
            if (Input.GetKeyDown(_key)) {
                string filename = string.Format("{0}.{1}",string.Format(_format, DateTime.UtcNow.ToString(_dateTimeFormat)), "png");
                string path = Path.Combine(Application.dataPath, filename);

                Debug.Log(string.Format("Screenshot taken at: {0}", path));

                if (File.Exists(path))
                    File.Delete(path);

                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    Application.CaptureScreenshot(filename);
                else
                    Application.CaptureScreenshot(path);
            }
        }
    }
}
