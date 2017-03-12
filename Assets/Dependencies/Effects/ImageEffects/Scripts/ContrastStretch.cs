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
    [AddComponentMenu("Image Effects/Color Adjustments/Contrast Stretch")]
    public class ContrastStretch : MonoBehaviour {
        /// Adaptation speed - percents per frame, if playing at 30FPS.
        /// Default is 0.02 (2% each 1/30s).
        [Range(0.0001f, 1.0f)]
        public float adaptationSpeed = 0.02f;


        // To maintain adaptation levels over time, we need two 1x1 render textures
        // and ping-pong between them.
        RenderTexture[] adaptRenderTex = new RenderTexture[2];
        int curAdaptIndex = 0;

        /// The limit on the maximum luminance (0...1) - we won't go below this.
        [Range(0.0f, 1.0f)]
        public float limitMaximum = 0.6f;

        /// If our scene is really dark (or really bright), we might not want to
        /// stretch its contrast to the full range.
        /// limitMinimum=0, limitMaximum=1 is the same as not applying the effect at all.
        /// limitMinimum=1, limitMaximum=0 is always stretching colors to full range.
        /// The limit on the minimum luminance (0...1) - we won't go above this.
        [Range(0.0f, 1.0f)]
        public float limitMinimum = 0.2f;

        Material m_materialAdapt;
        Material m_materialApply;
        Material m_materialLum;
        Material m_materialReduce;

        // Adaptation shader - gradually "adapts" minimum/maximum luminances,
        // based on currently adapted 1x1 image and the actual 1x1 image of the current scene.
        public Shader shaderAdapt;

        // Final pass - stretches the color values of the original scene, based on currently
        // adpated minimum/maximum values.
        public Shader shaderApply;


        // Computes scene luminance (grayscale) image
        public Shader shaderLum;

        // Reduces size of the image by 2x2, while computing maximum/minimum values.
        // By repeatedly applying this shader, we reduce the initial luminance image
        // to 1x1 image with minimum/maximum luminances found.
        public Shader shaderReduce;

        protected Material materialLum {
            get {
                if (m_materialLum == null) {
                    m_materialLum = new Material(shaderLum);
                    m_materialLum.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_materialLum;
            }
        }

        protected Material materialReduce {
            get {
                if (m_materialReduce == null) {
                    m_materialReduce = new Material(shaderReduce);
                    m_materialReduce.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_materialReduce;
            }
        }

        protected Material materialAdapt {
            get {
                if (m_materialAdapt == null) {
                    m_materialAdapt = new Material(shaderAdapt);
                    m_materialAdapt.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_materialAdapt;
            }
        }

        protected Material materialApply {
            get {
                if (m_materialApply == null) {
                    m_materialApply = new Material(shaderApply);
                    m_materialApply.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_materialApply;
            }
        }

        void Start() {
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects) {
                enabled = false;
                return;
            }

            if (!shaderAdapt.isSupported || !shaderApply.isSupported
                || !shaderLum.isSupported || !shaderReduce.isSupported) {
                enabled = false;
                return;
            }
        }

        void OnEnable() {
            for (var i = 0; i < 2; ++i) {
                if (!adaptRenderTex[i]) {
                    adaptRenderTex[i] = new RenderTexture(1, 1, 0);
                    adaptRenderTex[i].hideFlags = HideFlags.HideAndDontSave;
                }
            }
        }

        void OnDisable() {
            for (var i = 0; i < 2; ++i) {
                DestroyImmediate(adaptRenderTex[i]);
                adaptRenderTex[i] = null;
            }
            if (m_materialLum)
                DestroyImmediate(m_materialLum);
            if (m_materialReduce)
                DestroyImmediate(m_materialReduce);
            if (m_materialAdapt)
                DestroyImmediate(m_materialAdapt);
            if (m_materialApply)
                DestroyImmediate(m_materialApply);
        }


        /// Apply the filter
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            // Blit to smaller RT and convert to luminance on the way
            const int TEMP_RATIO = 1; // 4x4 smaller
            RenderTexture rtTempSrc =
                RenderTexture.GetTemporary(source.width / TEMP_RATIO,
                    source.height / TEMP_RATIO);
            Graphics.Blit(source, rtTempSrc, materialLum);

            // Repeatedly reduce this image in size, computing min/max luminance values
            // In the end we'll have 1x1 image with min/max luminances found.
            const int FINAL_SIZE = 1;
            //const int FINAL_SIZE = 1;
            while (rtTempSrc.width > FINAL_SIZE || rtTempSrc.height > FINAL_SIZE) {
                const int REDUCE_RATIO = 2; // our shader does 2x2 reduction
                int destW = rtTempSrc.width / REDUCE_RATIO;
                if (destW < FINAL_SIZE)
                    destW = FINAL_SIZE;
                int destH = rtTempSrc.height / REDUCE_RATIO;
                if (destH < FINAL_SIZE)
                    destH = FINAL_SIZE;
                RenderTexture rtTempDst = RenderTexture.GetTemporary(destW,
                    destH);
                Graphics.Blit(rtTempSrc, rtTempDst, materialReduce);

                // Release old src temporary, and make new temporary the source
                RenderTexture.ReleaseTemporary(rtTempSrc);
                rtTempSrc = rtTempDst;
            }

            // Update viewer's adaptation level
            CalculateAdaptation(rtTempSrc);

            // Apply contrast strech to the original scene, using currently adapted parameters
            materialApply.SetTexture("_AdaptTex", adaptRenderTex[curAdaptIndex]);
            Graphics.Blit(source, destination, materialApply);

            RenderTexture.ReleaseTemporary(rtTempSrc);
        }


        /// Helper function to do gradual adaptation to min/max luminances
        void CalculateAdaptation(Texture curTexture) {
            int prevAdaptIndex = curAdaptIndex;
            curAdaptIndex = (curAdaptIndex + 1) % 2;

            // Adaptation speed is expressed in percents/frame, based on 30FPS.
            // Calculate the adaptation lerp, based on current FPS.
            float adaptLerp = 1.0f
                - Mathf.Pow(1.0f - adaptationSpeed, 30.0f * Time.deltaTime);
            const float kMinAdaptLerp = 0.01f;
            adaptLerp = Mathf.Clamp(adaptLerp, kMinAdaptLerp, 1);

            materialAdapt.SetTexture("_CurTex", curTexture);
            materialAdapt.SetVector("_AdaptParams",
                new Vector4(adaptLerp, limitMinimum, limitMaximum, 0.0f));
            // clear destination RT so its contents don't need to be restored
            Graphics.SetRenderTarget(adaptRenderTex[curAdaptIndex]);
            GL.Clear(false, true, Color.black);
            Graphics.Blit(adaptRenderTex[prevAdaptIndex],
                adaptRenderTex[curAdaptIndex],
                materialAdapt);
        }
    }
}
