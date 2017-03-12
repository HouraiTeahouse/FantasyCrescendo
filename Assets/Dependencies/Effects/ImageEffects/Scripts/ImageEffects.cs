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

using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects {
    /// A Utility class for performing various image based rendering tasks.
    [AddComponentMenu("")]
    public class ImageEffects {
        public static void RenderDistortion(Material material,
                                            RenderTexture source,
                                            RenderTexture destination,
                                            float angle,
                                            Vector2 center,
                                            Vector2 radius) {
            bool invertY = source.texelSize.y < 0.0f;
            if (invertY) {
                center.y = 1.0f - center.y;
                angle = -angle;
            }

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero,
                Quaternion.Euler(0, 0, angle),
                Vector3.one);

            material.SetMatrix("_RotationMatrix", rotationMatrix);
            material.SetVector("_CenterRadius",
                new Vector4(center.x, center.y, radius.x, radius.y));
            material.SetFloat("_Angle", angle * Mathf.Deg2Rad);

            Graphics.Blit(source, destination, material);
        }


        [Obsolete("Use Graphics.Blit(source,dest) instead")]
        public static void Blit(RenderTexture source, RenderTexture dest) {
            Graphics.Blit(source, dest);
        }


        [Obsolete("Use Graphics.Blit(source, destination, material) instead")]
        public static void BlitWithMaterial(Material material,
                                            RenderTexture source,
                                            RenderTexture dest) {
            Graphics.Blit(source, dest, material);
        }
    }
}