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
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Camera/Tilt Shift (Lens Blur)")]
    internal class TiltShift : PostEffectsBase {
        public enum TiltShiftMode {
            TiltShiftMode,
            IrisMode,
        }

        public enum TiltShiftQuality {
            Preview,
            Normal,
            High,
        }

        [Range(0.0f, 15.0f)]
        public float blurArea = 1.0f;

        [Range(0, 1)]
        public int downsample = 0;

        [Range(0.0f, 25.0f)]
        public float maxBlurSize = 5.0f;

        public TiltShiftMode mode = TiltShiftMode.TiltShiftMode;
        public TiltShiftQuality quality = TiltShiftQuality.Normal;
        Material tiltShiftMaterial = null;

        public Shader tiltShiftShader = null;


        public override bool CheckResources() {
            CheckSupport(true);

            tiltShiftMaterial = CheckShaderAndCreateMaterial(tiltShiftShader,
                tiltShiftMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            tiltShiftMaterial.SetFloat("_BlurSize",
                maxBlurSize < 0.0f ? 0.0f : maxBlurSize);
            tiltShiftMaterial.SetFloat("_BlurArea", blurArea);
            source.filterMode = FilterMode.Bilinear;

            RenderTexture rt = destination;
            if (downsample > 0f) {
                rt = RenderTexture.GetTemporary(source.width >> downsample,
                    source.height >> downsample,
                    0,
                    source.format);
                rt.filterMode = FilterMode.Bilinear;
            }

            var basePassNr = (int) quality;
            basePassNr *= 2;
            Graphics.Blit(source,
                rt,
                tiltShiftMaterial,
                mode == TiltShiftMode.TiltShiftMode
                    ? basePassNr
                    : basePassNr + 1);

            if (downsample > 0) {
                tiltShiftMaterial.SetTexture("_Blurred", rt);
                Graphics.Blit(source, destination, tiltShiftMaterial, 6);
            }

            if (rt != destination)
                RenderTexture.ReleaseTemporary(rt);
        }
    }
}