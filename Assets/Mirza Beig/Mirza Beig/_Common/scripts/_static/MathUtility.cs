
// =================================	
// Namespaces.
// =================================

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

// =================================	
// Define namespace.
// =================================

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace Common
    {

        // =================================	
        // Classes.
        // =================================

        //[ExecuteInEditMode]
        [System.Serializable]

        public static class MathUtility
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            [System.Serializable]
            public class DualContainer<T>
            {
                public T a;
                public T b;
            }

            // Int range.

            [System.Serializable]
            public class RangeInt
            {
                public RangeInt()
                {
                    min = 0;
                    max = 1;
                }
                public RangeInt(int min, int max)
                {
                    this.min = min;
                    this.max = max;
                }

                public int random
                {
                    get { return Random.Range(min, max); }
                }
                public int getRandom()
                {
                    return Random.Range(min, max);
                }

                public int range
                {
                    get
                    {
                        return max - min;
                    }
                }

                public int getNormalized(int value)
                {
                    return value - min;
                }

                public override string ToString()
                {
                    return "(" + min.ToString() + ", " + max.ToString() + ")";
                }

                public int min = 0;
                public int max = 1;
            }

            // Float range.

            [System.Serializable]
            public class RangeFloat
            {
                public RangeFloat()
                {
                    min = 0.0f;
                    max = 1.0f;
                }
                public RangeFloat(float min, float max)
                {
                    this.min = min;
                    this.max = max;
                }

                public float getRandom()
                {
                    return Random.Range(min, max);
                }

                public float range
                {
                    get
                    {
                        return max - min;
                    }
                }

                public override string ToString()
                {
                    return "(" + min.ToString() + ", " + max.ToString() + ")";
                }

                public float min = 0.0f;
                public float max = 1.0f;
            }

            // Double range.

            [System.Serializable]
            public class RangeDouble
            {
                public RangeDouble()
                {
                    min = 0.0f;
                    max = 1.0f;
                }
                public RangeDouble(double min, double max)
                {
                    this.min = min;
                    this.max = max;
                }

                public float getRandom()
                {
                    return Random.Range((float)min, (float)max);
                }

                public double range
                {
                    get
                    {
                        return max - min;
                    }
                }

                public override string ToString()
                {
                    return "(" + min.ToString() + ", " + max.ToString() + ")";
                }

                public double min = 0.0d;
                public double max = 1.0d;
            }

            // ...

            public class TimeStampedValue<T>
            {
                public TimeStampedValue(T value, float time)
                {
                    this.value = value;
                    this.time = time;
                }

                public T value;
                public float time = 0.0f;
            }

            // Time-based buffer.

            [System.Serializable]
            public class Buffer<T>
            {
                //public Buffer(float bufferTime)
                //{
                //    this.bufferTime = bufferTime;
                //}

                // ...

                public TimeStampedValue<T> getFirstElement()
                {
                    return buffer.Count != 0 ? buffer[0] : null;
                }

                // ...

                public virtual void add(T value, float time)
                {
                    buffer.Add(new TimeStampedValue<T>(value, time));
                }

                // ...

                public virtual void update(float currentTime)
                {
                    if (buffer.Count != 0)
                    {
                        float oldestElementTime = 0.0f;
                        bool removeOldestElement = false;

                        do
                        {
                            oldestElementTime = buffer[0].time;
                            float timeSinceOldestElement = Mathf.Abs(currentTime - oldestElementTime);

                            removeOldestElement = timeSinceOldestElement > bufferTime;

                            if (removeOldestElement)
                            {
                                buffer.RemoveAt(0);
                            }

                        } while (removeOldestElement && buffer.Count != 0);
                    }
                }

                // ...

                public float bufferTime = 1.0f; // Seconds to record.
                protected List<TimeStampedValue<T>> buffer = new List<TimeStampedValue<T>>();
            }

            // Position and rotation container.

            [System.Serializable]
            public class TransformContainer
            {
                public Vector3 position;
                public Quaternion rotation;

                // ...

                public TransformContainer()
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                }

                public TransformContainer(Vector3 position, Quaternion rotation)
                {
                    set(position, rotation);
                }
                public TransformContainer(TransformContainer transformContainer)
                {
                    set(transformContainer);
                }

                // ...

                public void set(Vector3 position, Quaternion rotation)
                {
                    this.position = position;
                    this.rotation = rotation;
                }
                public void set(TransformContainer transformContainer)
                {
                    position = transformContainer.position;
                    rotation = transformContainer.rotation;
                }

                // ...

                public void setRotationAround(Vector3 point, Vector3 axis, float angle)
                {
                    set(getRotationAround(point, axis, angle));
                }
            }

            // Enumeration of interpolation types.

            public enum LerpType
            {
                lerpLinear,

                lerpEaseInSine, lerpEaseOutSine, lerpEaseInOutSine,
                lerpEaseInQuad, lerpEaseOutQuad, lerpEaseInOutQuad
            }


            // =================================	
            // Variables.
            // =================================

            // Pre-calcs.

            public const float PI_DIV2 = Mathf.PI / 2.0f;
            public const float PI_MULT2 = Mathf.PI * 2.0f;

            public const float RAD2DEG = 180.0f / Mathf.PI;
            public const float DEG2RAD = Mathf.PI / 180.0f;

            // =================================	
            // Functions.
            // =================================

            // Double precision clamp.

            public static double clamp(double value, double min, double max)
            {
                return (value < min) ? min : ((value > max) ? max : value);
            }

            // Clamp angle.

            //public static float clampAngle(float angle, float min = -360.0f, float max = 360.0f)
            //{
            //    if (angle < min)
            //    {
            //        angle += 360.0f;
            //    }
            //    else if (angle > max)
            //    {
            //        angle -= 360.0f;
            //    }

            //    //angle = Mathf.Clamp(angle, -360.0f, 360.0f);

            //    return angle;
            //}

            // Clamp euler angle (example, clamping to -80 and 40 == 280 and 40).
            // This function correctly handles this problem.

            public static float clampAngle(float angle, float min, float max)
            {
                if (angle < 180.0f)
                {
                    return Mathf.Clamp(angle, 0.0f, Mathf.Max(min, max));
                }
                else
                { 
                    return Mathf.Clamp(angle, 360.0f + Mathf.Min(min, max), 360.0f);
                }
            }

            // Next highest power of two (includes self).

            public static int nextPowerOfTwo(int x)
            {
                if (x <= 1)
                {
                    return 1;
                }

                x--;

                x |= x >> 1;
                x |= x >> 2;
                x |= x >> 4;
                x |= x >> 8;
                x |= x >> 16;

                x++;

                return x;
            }

            // Easing equations adapted from Robert Penner's easing page.
            // http://www.robertpenner.com/easing/

            public static float lerp(float from, float to, float time)
            {
                return ((to - from) * time) + from;
            }
            public static Vector3 lerp(Vector3 from, Vector3 to, float time)
            {
                return ((to - from) * time) + from;
            }

            // Pick an interpolation method.

            public static float lerp(float from, float to, float time, LerpType mode)
            {
                switch (mode)
                {
                    case LerpType.lerpLinear:
                        {
                            return (lerp(from, to, time));
                        }
                    case LerpType.lerpEaseInSine:
                        {
                            return (lerpEaseInSine(from, to, time));
                        }
                    case LerpType.lerpEaseOutSine:
                        {
                            return (lerpEaseOutSine(from, to, time));
                        }
                    case LerpType.lerpEaseInOutSine:
                        {
                            return (lerpEaseInOutSine(from, to, time));
                        }
                    case LerpType.lerpEaseInQuad:
                        {
                            return (lerpEaseInQuad(from, to, time));
                        }
                    case LerpType.lerpEaseOutQuad:
                        {
                            return (lerpEaseOutQuad(from, to, time));
                        }
                    case LerpType.lerpEaseInOutQuad:
                        {
                            return (lerpEaseInOutQuad(from, to, time));
                        }
                    default:
                        {
                            throw new System.ArgumentException("Unknown case.");
                        }
                }
            }
            public static Vector3 lerp(Vector3 from, Vector3 to, float time, LerpType mode)
            {
                switch (mode)
                {
                    case LerpType.lerpLinear:
                        {
                            return (lerp(from, to, time));
                        }
                    case LerpType.lerpEaseInSine:
                        {
                            return (lerpEaseInSine(from, to, time));
                        }
                    case LerpType.lerpEaseOutSine:
                        {
                            return (lerpEaseOutSine(from, to, time));
                        }
                    case LerpType.lerpEaseInOutSine:
                        {
                            return (lerpEaseInOutSine(from, to, time));
                        }
                    case LerpType.lerpEaseInQuad:
                        {
                            return (lerpEaseInQuad(from, to, time));
                        }
                    case LerpType.lerpEaseOutQuad:
                        {
                            return (lerpEaseOutQuad(from, to, time));
                        }
                    case LerpType.lerpEaseInOutQuad:
                        {
                            return (lerpEaseInOutQuad(from, to, time));
                        }
                    default:
                        {
                            throw new System.ArgumentException("Unknown case.");
                        }
                }
            }

            // ...

            public static float lerpEaseInSine(float from, float to, float time)
            {
                to -= from;
                return ((-to * Mathf.Cos(time * PI_DIV2)) + to) + from;
            }
            public static float lerpEaseOutSine(float from, float to, float time)
            {
                to -= from;
                return (to * Mathf.Sin(time * PI_DIV2)) + from;
            }
            public static float lerpEaseInOutSine(float from, float to, float time)
            {
                to -= from;
                return ((-to / 2.0f) * (Mathf.Cos(Mathf.PI * time) - 1.0f)) + from;
            }

            public static Vector3 lerpEaseInSine(Vector3 from, Vector3 to, float time)
            {
                to -= from;
                return ((-to * Mathf.Cos(time * PI_DIV2)) + to) + from;
            }
            public static Vector3 lerpEaseOutSine(Vector3 from, Vector3 to, float time)
            {
                to -= from;
                return (to * Mathf.Sin(time * PI_DIV2)) + from;
            }
            public static Vector3 lerpEaseInOutSine(Vector3 from, Vector3 to, float time)
            {
                to -= from;
                return ((-to / 2.0f) * (Mathf.Cos(Mathf.PI * time) - 1.0f)) + from;
            }

            // ...

            public static float lerpEaseInQuad(float from, float to, float time)
            {
                return ((to - from) * (time * time)) + from;
            }
            public static float lerpEaseOutQuad(float from, float to, float time)
            {
                return (-(to - from) * (time * (time - 2.0f))) + from;
            }
            public static float lerpEaseInOutQuad(float from, float to, float time)
            {
                if ((time *= 2.0f) < 1.0f)
                {
                    return (((to - from) / 2.0f) * (time * time)) + from;
                }

                return ((-(to - from) / 2.0f) * (((--time) * (time - 2.0f)) - 1.0f)) + from;
            }

            public static Vector3 lerpEaseInQuad(Vector3 from, Vector3 to, float time)
            {
                return ((to - from) * (time * time)) + from;
            }
            public static Vector3 lerpEaseOutQuad(Vector3 from, Vector3 to, float time)
            {
                return (-(to - from) * (time * (time - 2.0f))) + from;
            }
            public static Vector3 lerpEaseInOutQuad(Vector3 from, Vector3 to, float time)
            {
                if ((time *= 2.0f) < 1.0f)
                {
                    return (((to - from) / 2.0f) * (time * time)) + from;
                }

                return ((-(to - from) / 2.0f) * (((--time) * (time - 2.0f)) - 1.0f)) + from;
            }

            // ...

            //public static Vector3 lerpEaseInQuad(Vector3 from, Vector3 to, float time)
            //{
            //    time = time * time;
            //    return lerp(from, to, time);
            //}
            //public static Vector3 lerpEaseOutQuad(Vector3 from, Vector3 to, float time)
            //{
            //    time = time * time * time;
            //    return lerp(from, to, time);
            //}
            //public static Vector3 lerpEaseInOutQuad(Vector3 from, Vector3 to, float time)
            //{
            //    time = (3 * (time * time)) - (2 * (time * time * time));
            //    return lerp(from, to, time);
            //}

            // ...

            // Remap to range.

            public static float remap(float value, float fromMin, float fromMax, float toMin, float toMax, bool clamp = true)
            {
                float ret = (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;

                if (clamp)
                {
                    // Check which value is min and max, in case "to" values are not 
                    // toMin = smaller number and toMax = larger number. 

                    // Like if I was remapping [0.0f, 1.0f] -> [5.0f, 2.0f].

                    ret = Mathf.Clamp(ret, Mathf.Min(toMin, toMax), Mathf.Max(toMin, toMax));
                }

                return ret;
            }
            public static float remap(float value, RangeFloat from, RangeFloat to, bool clamp = true)
            {
                return remap(value, from.min, from.max, to.min, to.max, clamp);
            }

            public static Vector3 remap(Vector3 value, Vector3 fromMin, Vector3 fromMax, Vector3 toMin, Vector3 toMax, bool clamp = true)
            {
                Vector3 ret;

                ret.x = remap(value.x, fromMin.x, fromMax.x, toMin.x, toMax.x, clamp);
                ret.y = remap(value.y, fromMin.y, fromMax.y, toMin.y, toMax.y, clamp);
                ret.z = remap(value.z, fromMin.z, fromMax.z, toMin.z, toMax.z, clamp);

                return ret;
            }

            // Remap a range to 0.0f and 1.0f.

            public static float normalizeRange(float value, float min, float max, bool clamp = true)
            {
                return remap(value, min, max, 0.0f, 1.0f, clamp);
            }
            public static float normalizeRange(float value, RangeFloat range, bool clamp = true)
            {
                return normalizeRange(value, range.min, range.max, clamp);
            }

            public static Vector3 normalizeRange(Vector3 value, Vector3 min, Vector3 max, bool clamp = true)
            {
                return remap(value, min, max, Vector3.zero, Vector3.one, clamp);
            }

            //public static float remap(float value, float min1, float max1, float min2, float max2)
            //{
            //    return (value - min1) / (max1 - min1) * (max2 - min2) + min2;
            //}

            // Return a spline point based on catmull-rom interpolation and time(t).

            public static float catmullRom(float p1, float p2, float p3, float p4, float time)
            {
                float t2 = time * time;
                float t3 = t2 * time;

                return
                    0.5f * ((2 * p2) + (-p1 + p3) * time +
                    (2 * p1 - 5 * p2 + 4 * p3 - p4) * t2 +
                    (-p1 + 3 * p2 - 3 * p3 + p4) * t3);
            }

            // For floating point inaccuracies. Use this for comparison instead.

            public static bool floatEquals(float a, float b, float epsilon = 0.02f)
            {
                return Mathf.Abs(a - b) < epsilon;
            }

            // Rotate around a point by angle. Same as Unity's Transform.RotateAround().
            // Here for reference...

            public static void rotateAround(Transform transform, Vector3 point, Vector3 axis, float angle)
            {
                Vector3 position = transform.position;
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);

                Vector3 offset = position - point;

                offset = rotation * offset;
                position = point + offset;

                transform.position = position;
                transform.rotation *= rotation;
            }

            // Get transform rotation and position around a point. Not additive like Unity's RotateAround().

            public static TransformContainer getRotationAround(Transform transform, Vector3 point, Vector3 axis, float angle)
            {
                TransformContainer ret = new TransformContainer();

                ret.position = transform.position;
                ret.rotation = Quaternion.AngleAxis(angle, axis);

                Vector3 offset = ret.position - point;

                offset = ret.rotation * offset;
                ret.position = point + offset;

                return ret;
            }
            public static TransformContainer getRotationAround(Vector3 point, Vector3 axis, float angle)
            {
                TransformContainer ret = new TransformContainer();

                ret.position = Vector3.zero;
                ret.rotation = Quaternion.AngleAxis(angle, axis);

                Vector3 offset = ret.position - point;

                offset = ret.rotation * offset;
                ret.position = point + offset;

                return ret;
            }

            // Extension method version of getRotationAround() above.

            public static void setRotationAround(this Transform transform, Vector3 point, Vector3 axis, float angle, Space positionRelativeTo = Space.Self, Space rotationRelativeTo = Space.Self)
            {
                TransformContainer val = getRotationAround(point, axis, angle);

                if (positionRelativeTo == Space.Self)

                { transform.localPosition = val.position; }
                else { transform.position = val.position; }

                if (rotationRelativeTo == Space.Self)

                { transform.localRotation = val.rotation; }
                else { transform.rotation = val.rotation; }
            }
            public static void setRotationAround2(this Transform transform, Vector3 point, Vector3 axis, float angle, Space positionRelativeTo = Space.World, Space rotationRelativeTo = Space.World)
            {
                TransformContainer ret = new TransformContainer();

                ret.position = transform.position;
                ret.rotation = Quaternion.AngleAxis(angle, axis);

                Vector3 offset = ret.position - point;
                offset = ret.rotation * offset;

                ret.position = point + offset;

                Quaternion lookAtCenter = transform.rotation;
                ret.rotation = lookAtCenter * (Quaternion.Inverse(lookAtCenter) * ret.rotation * lookAtCenter);

                // ...

                if (positionRelativeTo == Space.Self)

                { transform.localPosition = ret.position; }
                else { transform.position = ret.position; }

                if (rotationRelativeTo == Space.Self)

                { transform.localRotation = ret.rotation; }
                else { transform.rotation = ret.rotation; }
            }

            // Extension for custom lerp from current transform values to target (pseudo-)transform values.

            public static void LerpTo(this Transform transform, TransformContainer target, float t, Space relativeTo = Space.Self)
            {
                if (relativeTo == Space.Self)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, target.position, t);
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, target.rotation, t);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.localPosition, target.position, t);
                    transform.rotation = Quaternion.Slerp(transform.localRotation, target.rotation, t);
                }
            }

            // Rotate around from a point around an axis (the pivot point).

            public static Vector3 rotatePointAroundPivot(
                Vector3 point, Vector3 axis, float angle)
            {
                return Quaternion.AngleAxis(angle, axis) * (point - axis) + axis;
            }
            public static Vector3 rotatePointAroundPivot(
                Vector3 point, Vector3 axis, Quaternion rotation)
            {
                return rotation * (point - axis) + axis;
            }

            // ...

            public static Vector2 getPointOnCircle(float angle)
            {
                float angleInRadians = Mathf.Deg2Rad * angle;
                return new Vector2(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians));
            }

            // Get yaw, pitch, and roll from a quaternion. Avoids gimbal lock.

            /*
             * Where xyzw are from a Quaternion. 
             * 
             *  yaw     = Mathf.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z);
             *  pitch   = Mathf.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z);
             *  roll    =  Mathf.Asin(2 * x * y + 2 * z * w);
             *  
            */

            public static float getYaw(Quaternion rotation)
            {
                return Mathf.Rad2Deg * Mathf.Atan2(
                    (2.0f * rotation.y * rotation.w) - (2.0f * rotation.x * rotation.z), 1.0f - (2.0f * rotation.y * rotation.y) - (2.0f * rotation.z * rotation.z));
            }
            public static float getPitch(Quaternion rotation)
            {
                return Mathf.Rad2Deg * Mathf.Atan2(
                    (2.0f * rotation.x * rotation.w) - (2.0f * rotation.y * rotation.z), 1.0f - (2.0f * rotation.x * rotation.x) - (2.0f * rotation.z * rotation.z));
            }
            public static float getRoll(Quaternion rotation)
            {
                return Mathf.Rad2Deg * Mathf.Asin(
                    (2.0f * rotation.x * rotation.y) + (2.0f * rotation.z * rotation.w));
            }

            public static Vector3 getYawPitchRoll(Quaternion rotation)
            {
                return new Vector3(getYaw(rotation), getPitch(rotation), getRoll(rotation));
            }

            // Get screen to world position.

            public static Vector3 mouseScreenToWorld(Camera camera)
            {
                Vector3 mousePosition = Input.mousePosition;

                mousePosition.z = -camera.transform.localPosition.z;
                return camera.ScreenToWorldPoint(mousePosition);
            }
            public static Vector3 mouseScreenToWorldSimulated(Vector3 position, Camera camera)
            {
                Vector3 mousePosition = position;

                mousePosition.z = -camera.transform.localPosition.z;
                return camera.ScreenToWorldPoint(mousePosition);
            }

            // =================================	
            // End functions.
            // =================================

        }

        // =================================	
        // End namespace.
        // =================================

    }

}

// =================================	
// --END-- //
// =================================
