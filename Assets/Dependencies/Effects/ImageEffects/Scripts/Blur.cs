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
    [AddComponentMenu("Image Effects/Blur/Blur")]
    public class Blur : MonoBehaviour {
        static Material m_Material = null;


        // --------------------------------------------------------
        // The blur iteration shader.
        // Basically it just takes 4 texture samples and averages them.
        // By applying it repeatedly and spreading out sample locations
        // we get a Gaussian blur approximation.

        public Shader blurShader = null;

        /// Blur spread for each iteration. Lower values
        /// give better looking blur, but require more iterations to
        /// get large blurs. Value is usually between 0.5 and 1.0.
        [Range(0.0f, 1.0f)]
        public float blurSpread = 0.6f;

        /// Blur iterations - larger number means more blur.
        [Range(0, 10)]
        public int iterations = 3;

        protected Material material {
            get {
                if (m_Material == null) {
                    m_Material = new Material(blurShader);
                    m_Material.hideFlags = HideFlags.DontSave;
                }
                return m_Material;
            }
        }

        protected void OnDisable() {
            if (m_Material) {
                DestroyImmediate(m_Material);
            }
        }

        // --------------------------------------------------------

        protected void Start() {
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects) {
                enabled = false;
                return;
            }
            // Disable if the shader can't run on the users graphics card
            if (!blurShader || !material.shader.isSupported) {
                enabled = false;
                return;
            }
        }

        // Performs one blur iteration.
        public void FourTapCone(RenderTexture source,
                                RenderTexture dest,
                                int iteration) {
            float off = 0.5f + iteration * blurSpread;
            Graphics.BlitMultiTap(source,
                dest,
                material,
                new Vector2(-off, -off),
                new Vector2(-off, off),
                new Vector2(off, off),
                new Vector2(off, -off));
        }

        // Downsamples the texture to a quarter resolution.
        void DownSample4x(RenderTexture source, RenderTexture dest) {
            var off = 1.0f;
            Graphics.BlitMultiTap(source,
                dest,
                material,
                new Vector2(-off, -off),
                new Vector2(-off, off),
                new Vector2(off, off),
                new Vector2(off, -off));
        }

        // Called by the camera to apply the image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            int rtW = source.width / 4;
            int rtH = source.height / 4;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, buffer);

            // Blur the small texture
            for (var i = 0; i < iterations; i++) {
                RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
                FourTapCone(buffer, buffer2, i);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;
            }
            Graphics.Blit(buffer, destination);

            RenderTexture.ReleaseTemporary(buffer);
        }
    }
}
