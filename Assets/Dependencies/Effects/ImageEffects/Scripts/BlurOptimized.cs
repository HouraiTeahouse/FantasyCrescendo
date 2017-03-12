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
    [AddComponentMenu("Image Effects/Blur/Blur (Optimized)")]
    public class BlurOptimized : PostEffectsBase {
        public enum BlurType {
            StandardGauss = 0,
            SgxGauss = 1,
        }

        [Range(1, 4)]
        public int blurIterations = 2;

        Material blurMaterial = null;

        public Shader blurShader = null;

        [Range(0.0f, 10.0f)]
        public float blurSize = 3.0f;

        public BlurType blurType = BlurType.StandardGauss;

        [Range(0, 2)]
        public int downsample = 1;


        public override bool CheckResources() {
            CheckSupport(false);

            blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        public void OnDisable() {
            if (blurMaterial)
                DestroyImmediate(blurMaterial);
        }

        public void OnRenderImage(RenderTexture source,
                                  RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            float widthMod = 1.0f / (1.0f * (1 << downsample));

            blurMaterial.SetVector("_Parameter",
                new Vector4(blurSize * widthMod,
                    -blurSize * widthMod,
                    0.0f,
                    0.0f));
            source.filterMode = FilterMode.Bilinear;

            int rtW = source.width >> downsample;
            int rtH = source.height >> downsample;

            // downsample
            RenderTexture rt = RenderTexture.GetTemporary(rtW,
                rtH,
                0,
                source.format);

            rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, rt, blurMaterial, 0);

            int passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

            for (var i = 0; i < blurIterations; i++) {
                float iterationOffs = i * 1.0f;
                blurMaterial.SetVector("_Parameter",
                    new Vector4(blurSize * widthMod + iterationOffs,
                        -blurSize * widthMod - iterationOffs,
                        0.0f,
                        0.0f));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW,
                    rtH,
                    0,
                    source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, blurMaterial, 1 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                // horizontal blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, blurMaterial, 2 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            Graphics.Blit(rt, destination);

            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
