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
    [AddComponentMenu("Image Effects/Edge Detection/Edge Detection")]
    public class EdgeDetection : PostEffectsBase {
        public enum EdgeDetectMode {
            TriangleDepthNormals = 0,
            RobertsCrossDepthNormals = 1,
            SobelDepth = 2,
            SobelDepthThin = 3,
            TriangleLuminance = 4,
        }

        Material edgeDetectMaterial = null;

        public Shader edgeDetectShader;
        public float edgeExp = 1.0f;
        public float edgesOnly = 0.0f;
        public Color edgesOnlyBgColor = Color.white;
        public float lumThreshold = 0.2f;


        public EdgeDetectMode mode = EdgeDetectMode.SobelDepthThin;
        EdgeDetectMode oldMode = EdgeDetectMode.SobelDepthThin;
        public float sampleDist = 1.0f;
        public float sensitivityDepth = 1.0f;
        public float sensitivityNormals = 1.0f;


        public override bool CheckResources() {
            CheckSupport(true);

            edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader,
                edgeDetectMaterial);
            if (mode != oldMode)
                SetCameraFlag();

            oldMode = mode;

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }


        new void Start() { oldMode = mode; }

        void SetCameraFlag() {
            if (mode == EdgeDetectMode.SobelDepth
                || mode == EdgeDetectMode.SobelDepthThin)
                GetComponent<Camera>().depthTextureMode |=
                    DepthTextureMode.Depth;
            else if (mode == EdgeDetectMode.TriangleDepthNormals
                || mode == EdgeDetectMode.RobertsCrossDepthNormals)
                GetComponent<Camera>().depthTextureMode |=
                    DepthTextureMode.DepthNormals;
        }

        void OnEnable() { SetCameraFlag(); }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources() == false) {
                Graphics.Blit(source, destination);
                return;
            }

            var sensitivity = new Vector2(sensitivityDepth, sensitivityNormals);
            edgeDetectMaterial.SetVector("_Sensitivity",
                new Vector4(sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
            edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
            edgeDetectMaterial.SetFloat("_SampleDistance", sampleDist);
            edgeDetectMaterial.SetVector("_BgColor", edgesOnlyBgColor);
            edgeDetectMaterial.SetFloat("_Exponent", edgeExp);
            edgeDetectMaterial.SetFloat("_Threshold", lumThreshold);

            Graphics.Blit(source, destination, edgeDetectMaterial, (int) mode);
        }
    }
}