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
    [AddComponentMenu("Image Effects/Camera/Vignette and Chromatic Aberration")]
    public class VignetteAndChromaticAberration : PostEffectsBase {
        public enum AberrationMode {
            Simple = 0,
            Advanced = 1,
        }

        public float axialAberration = 0.5f;
        public float blur = 0.0f; // blur == 0 disables blur pass (optimization)
        public float blurDistance = 2.5f;
        public float blurSpread = 0.75f;
        public Shader chromAberrationShader;
        // intensity == 0 disables pre pass (optimization)

        public float chromaticAberration = 0.2f;

        public float intensity = 0.036f;
        public float luminanceDependency = 0.25f;
        Material m_ChromAberrationMaterial;
        Material m_SeparableBlurMaterial;


        Material m_VignetteMaterial;


        public AberrationMode mode = AberrationMode.Simple;
        public Shader separableBlurShader;
        public Shader vignetteShader;


        public override bool CheckResources() {
            CheckSupport(false);

            m_VignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader,
                m_VignetteMaterial);
            m_SeparableBlurMaterial =
                CheckShaderAndCreateMaterial(separableBlurShader,
                    m_SeparableBlurMaterial);
            m_ChromAberrationMaterial =
                CheckShaderAndCreateMaterial(chromAberrationShader,
                    m_ChromAberrationMaterial);

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

            bool doPrepass = Mathf.Abs(blur) > 0.0f
                || Mathf.Abs(intensity) > 0.0f;

            float widthOverHeight = 1.0f * rtW / (1.0f * rtH);
            const float oneOverBaseSize = 1.0f / 512.0f;

            RenderTexture color = null;
            RenderTexture color2A = null;

            if (doPrepass) {
                color = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

                // Blur corners
                if (Mathf.Abs(blur) > 0.0f) {
                    color2A = RenderTexture.GetTemporary(rtW / 2,
                        rtH / 2,
                        0,
                        source.format);

                    Graphics.Blit(source, color2A, m_ChromAberrationMaterial, 0);

                    for (var i = 0; i < 2; i++) { // maybe make iteration count tweakable
                        m_SeparableBlurMaterial.SetVector("offsets",
                            new Vector4(0.0f,
                                blurSpread * oneOverBaseSize,
                                0.0f,
                                0.0f));
                        RenderTexture color2B =
                            RenderTexture.GetTemporary(rtW / 2,
                                rtH / 2,
                                0,
                                source.format);
                        Graphics.Blit(color2A, color2B, m_SeparableBlurMaterial);
                        RenderTexture.ReleaseTemporary(color2A);

                        m_SeparableBlurMaterial.SetVector("offsets",
                            new Vector4(
                                blurSpread * oneOverBaseSize / widthOverHeight,
                                0.0f,
                                0.0f,
                                0.0f));
                        color2A = RenderTexture.GetTemporary(rtW / 2,
                            rtH / 2,
                            0,
                            source.format);
                        Graphics.Blit(color2B, color2A, m_SeparableBlurMaterial);
                        RenderTexture.ReleaseTemporary(color2B);
                    }
                }

                m_VignetteMaterial.SetFloat("_Intensity",
                    1.0f / (1.0f - intensity) - 1.0f);
                // intensity for vignette
                m_VignetteMaterial.SetFloat("_Blur", 1.0f / (1.0f - blur) - 1.0f);
                    // blur intensity
                m_VignetteMaterial.SetTexture("_VignetteTex", color2A);
                // blurred texture

                Graphics.Blit(source, color, m_VignetteMaterial, 0);
                // prepass blit: darken & blur corners
            }

            m_ChromAberrationMaterial.SetFloat("_ChromaticAberration",
                chromaticAberration);
            m_ChromAberrationMaterial.SetFloat("_AxialAberration",
                axialAberration);
            m_ChromAberrationMaterial.SetVector("_BlurDistance",
                new Vector2(-blurDistance, blurDistance));
            m_ChromAberrationMaterial.SetFloat("_Luminance",
                1.0f / Mathf.Max(Mathf.Epsilon, luminanceDependency));

            if (doPrepass)
                color.wrapMode = TextureWrapMode.Clamp;
            else
                source.wrapMode = TextureWrapMode.Clamp;
            Graphics.Blit(doPrepass ? color : source,
                destination,
                m_ChromAberrationMaterial,
                mode == AberrationMode.Advanced ? 2 : 1);

            RenderTexture.ReleaseTemporary(color);
            RenderTexture.ReleaseTemporary(color2A);
        }
    }
}