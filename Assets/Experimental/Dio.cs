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

using UnityEngine;

namespace HouraiTeahouse {
    [ExecuteInEditMode]
    public class Dio : MonoBehaviour {
        [SerializeField]
        [Range(0, 1)]
        float _innerRatio;

        Material _mat;

        [SerializeField]
        [Range(0, 1)]
        float _outerRatio;

        [SerializeField, HideInInspector]
        Shader _shader;

        [SerializeField]
        Vector2 center;

        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (_mat == null) {
                if (_shader == null) {
                    enabled = false;
                    return;
                }
                _mat = new Material(_shader);
            }

            float aspectRatio = Screen.width / (float) Screen.height;

            _mat.SetVector("_Aspect", new Vector4(aspectRatio, 1, 1, 1));
            _mat.SetVector("_Center",
                new Vector4(center.x, center.y, _innerRatio, _outerRatio));

            Graphics.Blit(src, dst, _mat);
        }
    }
}
