using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse {

    /// <summary>
    /// A set of useful utility funcitons for the UnityEngine Rect struct 
    /// </summary>
    public static class RectExtensions {

        public static Rect ChangeWidth(this Rect rect, float width) {
            float diff = rect.width - width;
            rect.width = width;
            rect.x += diff / 2;
            return rect;
        }

        public static Rect ChangeHeight(this Rect rect, float height) {
            float diff = rect.height - height;
            rect.height = height;
            rect.y += diff / 2;
            return rect;
        }

        public static Rect ChangeDims(this Rect rect, float width, float height) {
            return rect.ChangeWidth(width).ChangeHeight(height);
        }

        public static Rect ChangeDims(this Rect rect, Vector2 dims) {
            return rect.ChangeDims(dims.x, dims.y);
        }

        public static float AspectRatio(this Rect rect) {
            if (rect.height == 0f)
                return float.NaN;
            return rect.width / rect.height;
        }

        public static Rect EnforceAspect(this Rect rect, float aspect) {
            if (rect.AspectRatio() < aspect) {
                // Image is wider than cropRect, extend it sideways
                return rect.ChangeWidth(aspect * rect.height);
            }
            // Image is wider than cropRect, extend it vertically
            return rect.ChangeHeight(rect.width / aspect);
        }

        public static Rect Restrict(this Rect rect, float width, float height, float? aspect = null) {
            Vector2 center = rect.center;
            if (aspect == null) {
                rect.width = Mathf.Min(rect.width, width);
                rect.height = Mathf.Min(rect.height, height);
            }
            else {
                if(height == 0)
                    throw new ArgumentException();
                float enclosingAspectRatio = width / height;
                float aspectRatio = aspect.Value;
                if (aspectRatio < enclosingAspectRatio) {
                    rect.width = width;
                    rect.height = width / aspectRatio;
                } else {
                    rect.height = height;
                    rect.width = height * aspectRatio;
                }
            }
            rect.center = center;
            return rect;
        }

    }
}
