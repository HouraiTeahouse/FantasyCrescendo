using UnityEngine;

namespace HouraiTeahouse {

    public static class TextureExtensions {

        public static float AspectRatio(this Texture texture) {
            Check.NotNull(texture);
            if (texture.height == 0)
                return float.NaN;
            return (float) texture.width / texture.height;
        }

        public static Vector2 Center(this Texture texture) { return texture.Size() / 2; }

        public static Vector2 Size(this Texture texture) {
            Check.NotNull(texture);
            return new Vector2(texture.width, texture.height);
        }

        public static Rect PixelRect(this Texture texture) {
            Check.NotNull(texture);
            return new Rect(0, 0, texture.width, texture.height);
        }

        public static Rect UVToPixelRect(this Texture texture, Rect uvRect) {
            Check.NotNull(texture);
            return new Rect(texture.width * uvRect.x,
                texture.height * uvRect.y,
                texture.width * uvRect.width,
                texture.height * uvRect.height);
        }

        public static Rect PixelToUVRect(this Texture texture, Rect pixelRect) {
            Check.NotNull(texture);
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