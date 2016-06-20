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

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {
    [RequireComponent(typeof(Text))]
    public sealed class FPSCounter : MonoBehaviour {
        Text Counter;
        float deltaTime;
        float fps;
        float msec;
        string outputText;

        void Awake() {
            Counter = GetComponent<Text>();
            StartCoroutine(UpdateDisplay());
        }

        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
        }

        IEnumerator UpdateDisplay() {
            while (true) {
                yield return new WaitForSeconds(0.5f);
                Counter.text = "{0:0.0} ms ({1:0.} fps)".With(msec, fps);
            }
        }
    }
}