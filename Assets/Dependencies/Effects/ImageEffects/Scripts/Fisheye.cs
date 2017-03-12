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

namespace UnityStandardAssets.ImageEffects {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Displacement/Fisheye")]
    public class Fisheye : PostEffectsBase {
        Material fisheyeMaterial = null;

        public Shader fishEyeShader = null;

        [Range(0.0f, 1.5f)]
        public float strengthX = 0.05f;

        [Range(0.0f, 1.5f)]
        public float strengthY = 0.05f;


        public override bool CheckResources() {
            CheckSupport(false);
            fisheyeMaterial = CheckShaderAndCreateMaterial(fishEyeShader,
                fisheyeMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            float oneOverBaseSize = 80.0f / 512.0f;
            // to keep values more like in the old version of fisheye

            float ar = source.width * 1.0f / (source.height * 1.0f);

            fisheyeMaterial.SetVector("intensity",
                new Vector4(strengthX * ar * oneOverBaseSize,
                    strengthY * oneOverBaseSize,
                    strengthX * ar * oneOverBaseSize,
                    strengthY * oneOverBaseSize));
            Graphics.Blit(source, destination, fisheyeMaterial);
        }
    }
}