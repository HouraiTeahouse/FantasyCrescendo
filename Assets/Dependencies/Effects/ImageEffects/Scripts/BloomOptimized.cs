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
    [AddComponentMenu("Image Effects/Bloom and Glow/Bloom (Optimized)")]
    public class BloomOptimized : PostEffectsBase {
        public enum BlurType {
            Standard = 0,
            Sgx = 1,
        }

        public enum Resolution {
            Low = 0,
            High = 1,
        }

        [Range(1, 4)]
        public int blurIterations = 1;

        [Range(0.25f, 5.5f)]
        public float blurSize = 1.0f;

        public BlurType blurType = BlurType.Standard;
        Material fastBloomMaterial;

        public Shader fastBloomShader = null;

        [Range(0.0f, 2.5f)]
        public float intensity = 0.75f;

        Resolution resolution = Resolution.Low;

        [Range(0.0f, 1.5f)]
        public float threshold = 0.25f;


        public override bool CheckResources() {
            CheckSupport(false);

            fastBloomMaterial = CheckShaderAndCreateMaterial(fastBloomShader,
                fastBloomMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnDisable() {
            if (fastBloomMaterial)
                DestroyImmediate(fastBloomMaterial);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            int divider = resolution == Resolution.Low ? 4 : 2;
            float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

            fastBloomMaterial.SetVector("_Parameter",
                new Vector4(blurSize * widthMod, 0.0f, threshold, intensity));
            source.filterMode = FilterMode.Bilinear;

            int rtW = source.width / divider;
            int rtH = source.height / divider;

            // downsample
            RenderTexture rt = RenderTexture.GetTemporary(rtW,
                rtH,
                0,
                source.format);
            rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, rt, fastBloomMaterial, 1);

            int passOffs = blurType == BlurType.Standard ? 0 : 2;

            for (var i = 0; i < blurIterations; i++) {
                fastBloomMaterial.SetVector("_Parameter",
                    new Vector4(blurSize * widthMod + i * 1.0f,
                        0.0f,
                        threshold,
                        intensity));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW,
                    rtH,
                    0,
                    source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 2 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                // horizontal blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 3 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            fastBloomMaterial.SetTexture("_Bloom", rt);

            Graphics.Blit(source, destination, fastBloomMaterial, 0);

            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
