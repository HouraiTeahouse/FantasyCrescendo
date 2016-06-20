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

namespace HouraiTeahouse {
    public static class TextureExtensions {
        public static float AspectRatio(this Texture texture) {
            Check.NotNull("texture", texture);
            if (texture.height == 0)
                return float.NaN;
            return (float) texture.width / texture.height;
        }

        public static Vector2 Center(this Texture texture) {
            return texture.Size() / 2;
        }

        public static Vector2 Size(this Texture texture) {
            Check.NotNull("texture", texture);
            return new Vector2(texture.width, texture.height);
        }

        public static Rect PixelRect(this Texture texture) {
            Check.NotNull("texture", texture);
            return new Rect(0, 0, texture.width, texture.height);
        }

        public static Rect UVToPixelRect(this Texture texture, Rect uvRect) {
            Check.NotNull("texture", texture);
            return new Rect(texture.width * uvRect.x,
                texture.height * uvRect.y,
                texture.width * uvRect.width,
                texture.height * uvRect.height);
        }

        public static Rect PixelToUVRect(this Texture texture, Rect pixelRect) {
            Check.NotNull("texture", texture);
            var uvRect = new Rect();
            if (texture.width != 0) {
                uvRect.x = pixelRect.x / texture.width;
                uvRect.width = pixelRect.width / texture.width;
            }
            else {
                uvRect.x = 0;
                uvRect.width = 0;
            }
            if (texture.height != 0) {
                uvRect.y = pixelRect.y / texture.height;
                uvRect.height = pixelRect.height / texture.height;
            }
            else {
                uvRect.x = 0;
                uvRect.height = 0;
            }
            return uvRect;
        }
    }
}