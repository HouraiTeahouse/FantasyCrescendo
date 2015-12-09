using System;
using System.IO;
using UnityEngine;

namespace Hourai {

    /// <summary>
    /// Takes screenshots upon pressing a specified button
    /// </summary>
    public class Screenshot : MonoBehaviour {

        [SerializeField]
        private KeyCode _key = KeyCode.F12;

        private void Update() {
            if (!Input.GetKeyDown(_key))
                return;
            string filename = "screenshot-" + DateTime.UtcNow.ToString("MM-dd-yyyy-HHmmss") + ".png";
            string path = Path.Combine(Application.persistentDataPath, filename);

            if (File.Exists(path))
                File.Delete(path);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
                Application.CaptureScreenshot(filename);
            else
                Application.CaptureScreenshot(path);
        }

    }

}