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
    [AddComponentMenu("Image Effects/Rendering/Screen Space Ambient Obscurance")
    ]
    internal class ScreenSpaceAmbientObscurance : PostEffectsBase {
        Material aoMaterial = null;
        public Shader aoShader = null;

        [Range(0, 5)]
        public float blurFilterDistance = 1.25f;

        [Range(0, 3)]
        public int blurIterations = 1;

        [Range(0, 1)]
        public int downsample = 0;

        [Range(0, 3)]
        public float intensity = 0.5f;

        [Range(0.1f, 3)]
        public float radius = 0.2f;

        public Texture2D rand = null;

        public override bool CheckResources() {
            CheckSupport(true);

            aoMaterial = CheckShaderAndCreateMaterial(aoShader, aoMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnDisable() {
            if (aoMaterial)
                DestroyImmediate(aoMaterial);
            aoMaterial = null;
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            Matrix4x4 P = GetComponent<Camera>().projectionMatrix;
            Matrix4x4 invP = P.inverse;
            var projInfo = new Vector4(-2.0f / (Screen.width * P[0]),
                -2.0f / (Screen.height * P[5]),
                (1.0f - P[2]) / P[0],
                (1.0f + P[6]) / P[5]);

            aoMaterial.SetVector("_ProjInfo", projInfo);
            // used for unprojection
            aoMaterial.SetMatrix("_ProjectionInv", invP);
            // only used for reference
            aoMaterial.SetTexture("_Rand", rand); // not needed for DX11 :)
            aoMaterial.SetFloat("_Radius", radius);
            aoMaterial.SetFloat("_Radius2", radius * radius);
            aoMaterial.SetFloat("_Intensity", intensity);
            aoMaterial.SetFloat("_BlurFilterDistance", blurFilterDistance);

            int rtW = source.width;
            int rtH = source.height;

            RenderTexture tmpRt = RenderTexture.GetTemporary(rtW >> downsample,
                rtH >> downsample);
            RenderTexture tmpRt2;

            Graphics.Blit(source, tmpRt, aoMaterial, 0);

            if (downsample > 0) {
                tmpRt2 = RenderTexture.GetTemporary(rtW, rtH);
                Graphics.Blit(tmpRt, tmpRt2, aoMaterial, 4);
                RenderTexture.ReleaseTemporary(tmpRt);
                tmpRt = tmpRt2;

                // @NOTE: it's probably worth a shot to blur in low resolution
                //  instead with a bilat-upsample afterwards ...
            }

            for (var i = 0; i < blurIterations; i++) {
                aoMaterial.SetVector("_Axis", new Vector2(1.0f, 0.0f));
                tmpRt2 = RenderTexture.GetTemporary(rtW, rtH);
                Graphics.Blit(tmpRt, tmpRt2, aoMaterial, 1);
                RenderTexture.ReleaseTemporary(tmpRt);

                aoMaterial.SetVector("_Axis", new Vector2(0.0f, 1.0f));
                tmpRt = RenderTexture.GetTemporary(rtW, rtH);
                Graphics.Blit(tmpRt2, tmpRt, aoMaterial, 1);
                RenderTexture.ReleaseTemporary(tmpRt2);
            }

            aoMaterial.SetTexture("_AOTex", tmpRt);
            Graphics.Blit(source, destination, aoMaterial, 2);

            RenderTexture.ReleaseTemporary(tmpRt);
        }
    }
}