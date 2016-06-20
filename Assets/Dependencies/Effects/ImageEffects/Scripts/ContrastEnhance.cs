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
    [AddComponentMenu(
        "Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)")]
    public class ContrastEnhance : PostEffectsBase {
        [Range(0.0f, 1.0f)]
        public float blurSpread = 1.0f;

        Material contrastCompositeMaterial;
        public Shader contrastCompositeShader = null;

        [Range(0.0f, 1.0f)]
        public float intensity = 0.5f;

        Material separableBlurMaterial;

        public Shader separableBlurShader = null;

        [Range(0.0f, 0.999f)]
        public float threshold = 0.0f;


        public override bool CheckResources() {
            CheckSupport(false);

            contrastCompositeMaterial =
                CheckShaderAndCreateMaterial(contrastCompositeShader,
                    contrastCompositeMaterial);
            separableBlurMaterial =
                CheckShaderAndCreateMaterial(separableBlurShader,
                    separableBlurMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            int rtW = source.width;
            int rtH = source.height;

            RenderTexture color2 = RenderTexture.GetTemporary(rtW / 2,
                rtH / 2,
                0);

            // downsample

            Graphics.Blit(source, color2);
            RenderTexture color4a = RenderTexture.GetTemporary(rtW / 4,
                rtH / 4,
                0);
            Graphics.Blit(color2, color4a);
            RenderTexture.ReleaseTemporary(color2);

            // blur

            separableBlurMaterial.SetVector("offsets",
                new Vector4(0.0f, blurSpread * 1.0f / color4a.height, 0.0f, 0.0f));
            RenderTexture color4b = RenderTexture.GetTemporary(rtW / 4,
                rtH / 4,
                0);
            Graphics.Blit(color4a, color4b, separableBlurMaterial);
            RenderTexture.ReleaseTemporary(color4a);

            separableBlurMaterial.SetVector("offsets",
                new Vector4(blurSpread * 1.0f / color4a.width, 0.0f, 0.0f, 0.0f));
            color4a = RenderTexture.GetTemporary(rtW / 4, rtH / 4, 0);
            Graphics.Blit(color4b, color4a, separableBlurMaterial);
            RenderTexture.ReleaseTemporary(color4b);

            // composite

            contrastCompositeMaterial.SetTexture("_MainTexBlurred", color4a);
            contrastCompositeMaterial.SetFloat("intensity", intensity);
            contrastCompositeMaterial.SetFloat("threshold", threshold);
            Graphics.Blit(source, destination, contrastCompositeMaterial);

            RenderTexture.ReleaseTemporary(color4a);
        }
    }
}
