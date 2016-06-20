// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
            string filename =
                _format.With(DateTime.UtcNow.ToString(_dateTimeFormat)) + ".png";
            string path = Path.Combine(Application.dataPath, filename);

            Log.Info("Screenshot taken. Saved to {0}", path);

            if (File.Exists(path))
                File.Delete(path);

            Application.CaptureScreenshot(Application.platform
                == RuntimePlatform.IPhonePlayer
                ? filename
                : path);
        }
    }
}