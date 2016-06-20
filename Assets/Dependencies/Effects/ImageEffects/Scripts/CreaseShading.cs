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
    [AddComponentMenu("Image Effects/Edge Detection/Crease Shading")]
    public class CreaseShading : PostEffectsBase {
        Material blurMaterial = null;

        public Shader blurShader = null;
        Material creaseApplyMaterial = null;

        public Shader creaseApplyShader = null;
        Material depthFetchMaterial = null;

        public Shader depthFetchShader = null;
        public float intensity = 0.5f;
        public int softness = 1;
        public float spread = 1.0f;


        public override bool CheckResources() {
            CheckSupport(true);

            blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
            depthFetchMaterial = CheckShaderAndCreateMaterial(depthFetchShader,
                depthFetchMaterial);
            creaseApplyMaterial = CheckShaderAndCreateMaterial(
                creaseApplyShader,
                creaseApplyMaterial);

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

            float widthOverHeight = 1.0f * rtW / (1.0f * rtH);
            float oneOverBaseSize = 1.0f / 512.0f;

            RenderTexture hrTex = RenderTexture.GetTemporary(rtW, rtH, 0);
            RenderTexture lrTex1 = RenderTexture.GetTemporary(rtW / 2,
                rtH / 2,
                0);

            Graphics.Blit(source, hrTex, depthFetchMaterial);
            Graphics.Blit(hrTex, lrTex1);

            for (var i = 0; i < softness; i++) {
                RenderTexture lrTex2 = RenderTexture.GetTemporary(rtW / 2,
                    rtH / 2,
                    0);
                blurMaterial.SetVector("offsets",
                    new Vector4(0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
                Graphics.Blit(lrTex1, lrTex2, blurMaterial);
                RenderTexture.ReleaseTemporary(lrTex1);
                lrTex1 = lrTex2;

                lrTex2 = RenderTexture.GetTemporary(rtW / 2, rtH / 2, 0);
                blurMaterial.SetVector("offsets",
                    new Vector4(spread * oneOverBaseSize / widthOverHeight,
                        0.0f,
                        0.0f,
                        0.0f));
                Graphics.Blit(lrTex1, lrTex2, blurMaterial);
                RenderTexture.ReleaseTemporary(lrTex1);
                lrTex1 = lrTex2;
            }

            creaseApplyMaterial.SetTexture("_HrDepthTex", hrTex);
            creaseApplyMaterial.SetTexture("_LrDepthTex", lrTex1);
            creaseApplyMaterial.SetFloat("intensity", intensity);
            Graphics.Blit(source, destination, creaseApplyMaterial);

            RenderTexture.ReleaseTemporary(hrTex);
            RenderTexture.ReleaseTemporary(lrTex1);
        }
    }
}