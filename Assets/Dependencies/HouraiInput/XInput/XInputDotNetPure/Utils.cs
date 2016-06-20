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

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;

namespace XInputDotNetPure {
    internal static class Utils {
        public const uint Success = 0x000;
        public const uint NotConnected = 0x000;

        const int LeftStickDeadZone = 7849;
        const int RightStickDeadZone = 8689;
        const int TriggerDeadZone = 30;


        public static float ApplyTriggerDeadZone(byte value,
                                                 GamePadDeadZone deadZoneMode) {
            if (deadZoneMode == GamePadDeadZone.None) {
                return ApplyDeadZone(value, byte.MaxValue, 0.0f);
            }
            else {
                return ApplyDeadZone(value, byte.MaxValue, TriggerDeadZone);
            }
        }


        public static GamePadThumbSticks.StickValue ApplyLeftStickDeadZone(
            short valueX,
            short valueY,
            GamePadDeadZone deadZoneMode) {
            return ApplyStickDeadZone(valueX,
                valueY,
                deadZoneMode,
                LeftStickDeadZone);
        }


        public static GamePadThumbSticks.StickValue ApplyRightStickDeadZone(
            short valueX,
            short valueY,
            GamePadDeadZone deadZoneMode) {
            return ApplyStickDeadZone(valueX,
                valueY,
                deadZoneMode,
                RightStickDeadZone);
        }


        static GamePadThumbSticks.StickValue ApplyStickDeadZone(short valueX,
                                                                short valueY,
                                                                GamePadDeadZone
                                                                    deadZoneMode,
                                                                int deadZoneSize) {
            if (deadZoneMode == GamePadDeadZone.Circular) {
                // Cast to long to avoid int overflow if valueX and valueY are both 32768, which would result in a negative number and Sqrt returns NaN
                var distanceFromCenter =
                    (float)
                        Math.Sqrt((long) valueX * (long) valueX
                            + (long) valueY * (long) valueY);
                float coefficient = ApplyDeadZone(distanceFromCenter,
                    short.MaxValue,
                    deadZoneSize);
                coefficient = coefficient > 0.0f
                    ? coefficient / distanceFromCenter
                    : 0.0f;
                return
                    new GamePadThumbSticks.StickValue(
                        Clamp(valueX * coefficient),
                        Clamp(valueY * coefficient));
            }
            else if (deadZoneMode == GamePadDeadZone.IndependentAxes) {
                return
                    new GamePadThumbSticks.StickValue(
                        ApplyDeadZone(valueX, short.MaxValue, deadZoneSize),
                        ApplyDeadZone(valueY, short.MaxValue, deadZoneSize));
            }
            else {
                return
                    new GamePadThumbSticks.StickValue(
                        ApplyDeadZone(valueX, short.MaxValue, 0.0f),
                        ApplyDeadZone(valueY, short.MaxValue, 0.0f));
            }
        }


        static float Clamp(float value) {
            return value < -1.0f ? -1.0f : (value > 1.0f ? 1.0f : value);
        }


        static float ApplyDeadZone(float value,
                                   float maxValue,
                                   float deadZoneSize) {
            if (value < -deadZoneSize) {
                value += deadZoneSize;
            }
            else if (value > deadZoneSize) {
                value -= deadZoneSize;
            }
            else {
                return 0.0f;
            }

            value /= maxValue - deadZoneSize;

            return Clamp(value);
        }
    }
}

#endif