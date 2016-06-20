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

// This class implements simple ghosting type Motion Blur.
// If Extra Blur is selected, the scene will allways be a little blurred,
// as it is scaled to a smaller resolution.
// The effect works by accumulating the previous frames in an accumulation
// texture.

namespace UnityStandardAssets.ImageEffects {
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
    [RequireComponent(typeof(Camera))]
    public class MotionBlur : ImageEffectBase {
        RenderTexture accumTexture;

        [Range(0.0f, 0.92f)]
        public float blurAmount = 0.8f;

        public bool extraBlur = false;

        protected override void Start() {
            if (!SystemInfo.supportsRenderTextures) {
                enabled = false;
                return;
            }
            base.Start();
        }

        protected override void OnDisable() {
            base.OnDisable();
            DestroyImmediate(accumTexture);
        }

        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            // Create the accumulation texture
            if (accumTexture == null || accumTexture.width != source.width
                || accumTexture.height != source.height) {
                DestroyImmediate(accumTexture);
                accumTexture = new RenderTexture(source.width, source.height, 0);
                accumTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, accumTexture);
            }

            // If Extra Blur is selected, downscale the texture to 4x4 smaller resolution.
            if (extraBlur) {
                RenderTexture blurbuffer =
                    RenderTexture.GetTemporary(source.width / 4,
                        source.height / 4,
                        0);
                accumTexture.MarkRestoreExpected();
                Graphics.Blit(accumTexture, blurbuffer);
                Graphics.Blit(blurbuffer, accumTexture);
                RenderTexture.ReleaseTemporary(blurbuffer);
            }

            // Clamp the motion blur variable, so it can never leave permanent trails in the image
            blurAmount = Mathf.Clamp(blurAmount, 0.0f, 0.92f);

            // Setup the texture and floating point values in the shader
            material.SetTexture("_MainTex", accumTexture);
            material.SetFloat("_AccumOrig", 1.0F - blurAmount);

            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            accumTexture.MarkRestoreExpected();

            // Render the image using the motion blur shader
            Graphics.Blit(source, accumTexture, material);
            Graphics.Blit(accumTexture, destination);
        }
    }
}